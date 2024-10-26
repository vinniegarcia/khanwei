using System;
using System.Collections.Generic;

public class ConwaysGame
{
    private readonly Dictionary<Guid, bool[,]> boards = new();

    // Req 1: Allows uploading a new board state.
    // Also saves board state.
    // Returns: ID of board
    public Guid UploadBoard(bool[,] initialState)
    {
        Guid boardId = Guid.NewGuid();
        boards[boardId] = (bool[,])initialState.Clone();
        return boardId;
    }

    // Req 2: Get next state for a board.
    // Returns: next state
    public bool[,] GetNextState(Guid boardId)
    {
        if (!boards.ContainsKey(boardId))
            throw new Exception("Board not found.");

        var currentState = boards[boardId];
        var nextState = Tick(currentState);
        boards[boardId] = nextState;
        return nextState;
    }

    // Method 3: Gets x number of states away for board
    public bool[,] GetFutureState(Guid boardId, int steps)
    {
        if (!boards.ContainsKey(boardId))
            throw new Exception("Board not found.");

        var state = (bool[,])boards[boardId].Clone();
        for (int i = 0; i < steps; i++)
            state = Tick(state);

        return state;
    }

    // Method 4: Gets final state for board.
    // If not found after x attempts, returns an error
    public bool[,] GetFinalState(Guid boardId, int maxAttempts)
    {
        if (!boards.ContainsKey(boardId))
            throw new Exception("Board not found.");

        var previousStates = new HashSet<string>();
        var state = (bool[,])boards[boardId].Clone();

        for (int i = 0; i < maxAttempts; i++)
        {
            string stateHash = GetBoardHash(state);

            if (previousStates.Contains(stateHash))
                return state; // Stable or oscillating state reached

            previousStates.Add(stateHash);
            state = Tick(state);
        }

        throw new Exception("Board did not reach a final state within the specified attempts.");
    }

    // This is the "tick" that computes the next state,
    // setting the liveness of each cell.
    // Return: next state for that board.
    private bool[,] Tick(bool[,] board)
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
    private int CountLiveNeighbors(bool[,] board, int row, int col)
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
    private string GetBoardHash(bool[,] board)
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
