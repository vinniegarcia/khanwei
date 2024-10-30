using System.Text.Json;

public interface IConwayBoardService {
    Task<List<ConwayBoard>> GetBoards();
    Task<ConwayBoard> GetBoard(Guid id);
    Task<ConwayBoard> CreateBoard(ConwayBoard b);

    Task<List<List<bool>>> GetFutureState(Guid id, int steps);
    Task<List<List<bool>>> GetFinalState(Guid id, int maxAttempts);
}

public class ConwayBoardService: IConwayBoardService {
    private ConwayDatabase _db;

    public ConwayBoardService(string connectionString)
    {
        _db = new ConwayDatabase(connectionString);
    }
    
    public async Task<List<ConwayBoard>> GetBoards() 
    {
        return await _db.SelectAll();
    }

    public async Task<ConwayBoard> CreateBoard(ConwayBoard b) {
        bool[,] values = BoardSerializer.FromSerializableFormat(b.Board);
        var boardId = await _db.Insert(values);
        return await GetBoard(boardId);
    }

    public Task<ConwayBoard> GetBoard(Guid id) {
        return _db.Select(id);
    }

    public async Task<List<List<bool>>> GetFutureState(Guid id, int steps = 1) {
        var board = await this.GetBoard(id);
        return ConwaysGameMove.GetFutureState(board, steps);
    }

    public async Task<List<List<bool>>> GetFinalState(Guid id, int maxAttempts = 100) {
        var board = await this.GetBoard(id);
        return ConwaysGameMove.GetFinalState(board, maxAttempts);
    }

}