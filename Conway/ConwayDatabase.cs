using Microsoft.Data.Sqlite;
using System.Text.Json;

interface IConwayDatabase {
    void Create();
    Task<Guid> Insert(bool[,] values);
    Task<ConwayBoard> Select(Guid id);
    Task<List<ConwayBoard>> SelectAll();

}

class ConwayDatabase
{
    private string _connectionString;
    public ConwayDatabase(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Create()
    {
        using(var conn = new SqliteConnection(_connectionString))
        {
            conn.Open();
            string sql = "CREATE TABLE boards (id TEXT PRIMARY KEY, board TEXT UNIQUE)";
            SqliteCommand command = new SqliteCommand(sql, conn);
            command.ExecuteNonQueryAsync();
        }
    }

    public async Task<Guid> Insert(bool[,] values)
    {
        using (var conn = new SqliteConnection(_connectionString))
        {
            conn.Open();
            // check to see if we already have this board.
            // If we do, return with the existing guid.
            // If we don't, insert and set new guid.
            var boardSerialized = BoardSerializer.SerializeBoard(values);
            var checkCommand = new SqliteCommand(@"
                SELECT id FROM boards
                WHERE board=$board
            ", conn);
            checkCommand.Parameters.AddWithValue("$board", boardSerialized);
            string boardId;
            boardId = (string) await checkCommand.ExecuteScalarAsync();
            if (boardId != null) {
                return Guid.Parse(boardId);
            }
            var command = new SqliteCommand(@"
                INSERT INTO boards(id, board)
                VALUES($id, $board)
                RETURNING id
            ", conn);
            command.Parameters.AddWithValue("$id", Guid.NewGuid());
            command.Parameters.AddWithValue("$board", boardSerialized);
            return  Guid.Parse((string)await command.ExecuteScalarAsync());
        }
    }
    
    public async Task<ConwayBoard> Select(Guid id)
    {
        using (var conn = new SqliteConnection(_connectionString))
        {
            conn.Open();
            var command = new SqliteCommand(@"
                SELECT board
                FROM boards
                WHERE id=$id
            ", conn);
            command.Parameters.AddWithValue("$id", id);
            var board = (string)await command.ExecuteScalarAsync();
            if (board != null)
            {
                return new ConwayBoard(JsonSerializer.Deserialize<List<List<bool>>>(board), id);
            }
            throw new Exception("No board found for the given ID");
        }
    }

    public async Task<List<ConwayBoard>> SelectAll()
    {
        using (var conn = new SqliteConnection(_connectionString))
        {
            conn.Open();
            var command = new SqliteCommand(@"
                SELECT id, board
                FROM boards
            ", conn);
            var reader = await command.ExecuteReaderAsync();
            var conwayBoards = new List<ConwayBoard>();
            while (reader.Read())
            {
                var conBoard = BuildConwayBoard(reader);
                conwayBoards.Add(conBoard);
            }
            return conwayBoards;
        }
    }

    private ConwayBoard BuildConwayBoard(SqliteDataReader reader)
    {
        // turn sqlite row result into a typed ConwayBoard object
        // with proper conversions to guid/lists.
        var id = reader.GetString(0);
        // deserialize this json string out of sqlite
        var board = JsonSerializer.Deserialize<List<List<bool>>>(reader.GetString(1));
        if (board != null)
        {
            return new ConwayBoard(board, Guid.Parse(id));
        }
        throw new Exception("No board found for the given ID");
    }

}