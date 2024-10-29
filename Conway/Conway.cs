public record ConwayBoard(List<List<bool>> Board, Guid? Id);

public class ConwaysGame
{
    public readonly Dictionary<Guid, ConwayBoard> games = new();

    // Req 1: Allows uploading a new board state.
    // Also saves board state.
    // Returns: ID of board
    public Guid InitializeBoard(bool[,] initialState)
    {
        Guid boardId = Guid.NewGuid();
        games[boardId] = new ConwayBoard(BoardSerializer.ToSerializableFormat((bool[,])initialState.Clone()), boardId);
        return boardId;
    }

    public ConwayBoard GetBoard(Guid boardId)
    {
        if (!games.ContainsKey(boardId))
            throw new Exception("Board not found.");
        return games[boardId];
    }
}

public class ConwaysGameMove
{
    // Req 2: Get next state for a board.
    // Returns: next state
    public static List<List<bool>> GetNextState(ConwayBoard b)
    {
        return GetFutureState(b, 1);
    }

    // Method 3: Gets x number of states away for board
    public static List<List<bool>> GetFutureState(ConwayBoard b, int steps)
    {
        var state = BoardSerializer.FromSerializableFormat(b.Board);
        for (int i = 0; i < steps; i++)
            state = Tick(state);

        return BoardSerializer.ToSerializableFormat(state);
    }

    // Method 4: Gets final state for board.
    // If not found after x attempts, returns an error
    public static List<List<bool>> GetFinalState(ConwayBoard b, int maxAttempts)
    {
        var previousStates = new HashSet<string>();
        var state = BoardSerializer.FromSerializableFormat(b.Board);

        for (int i = 0; i < maxAttempts; i++)
        {
            string stateHash = GetBoardHash(state);

            if (previousStates.Contains(stateHash))
                return BoardSerializer.ToSerializableFormat(state); // Stable or oscillating state reached

            previousStates.Add(stateHash);
            state = Tick(state);
        }

        throw new Exception("Board did not reach a final state within the specified attempts.");
    }

    // This is the "tick" that computes the next state,
    // setting the liveness of each cell.
    // Return: next state for that board.
    private static bool[,] Tick(bool[,] board)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        var nextState = new bool[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int liveNeighbors = CountLiveNeighbors(board, row, col);
                if (board[row, col]) // Alive cell
                    /*
                    Any live cell with fewer than two live neighbours dies, as if by underpopulation.
                    Any live cell with two or three live neighbours lives on to the next generation.
                    Any live cell with more than three live neighbours dies, as if by overpopulation.
                    */
                    nextState[row, col] = liveNeighbors == 2 || liveNeighbors == 3;
                else // Dead cell
                    /*
                    Any dead cell with exactly three live neighbours becomes a live cell, 
                    as if by reproduction.
                    */
                    nextState[row, col] = liveNeighbors == 3;
            }
        }

        return nextState;
    }

    // Collect surrounding neighbor cells to compute live count.
    private static int CountLiveNeighbors(bool[,] board, int row, int col)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        int count = 0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; // Skip the cell itself
                int newRow = row + i;
                int newCol = col + j;

                if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols && board[newRow, newCol])
                    count++;
            }
        }

        return count;
    }

    // Give each board a unique ID to find later.
    private static string GetBoardHash(bool[,] board)
    {
        var hash = new System.Text.StringBuilder();
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
                hash.Append(board[row, col] ? '1' : '0');
        }

        return hash.ToString();
    }

}
