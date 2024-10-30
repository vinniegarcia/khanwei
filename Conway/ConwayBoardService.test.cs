using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

public class ConwayBoardServiceTests : IDisposable
{
    private readonly IConwayBoardService _service;
    private readonly string _dbPath;

    public ConwayBoardServiceTests()
    {
        // create a custom database for this test, that gets
        // deleted when the test completes.
        _dbPath = "./" + DateTime.Now.ToString("O") + ".db";
        var connectionString = "Data Source=" + _dbPath;
        var _db = new ConwayDatabase(connectionString);
        _db.Create();
        _service = new ConwayBoardService(connectionString);

        // insert sample row
        bool[,] values = {
            {true, false, true},
            {false, true, false},
            {true, false, true}
        };   
        _db.Insert(values);
    }

    public void Dispose()
    {
        // Dispose of test sqlite db
        File.Delete(_dbPath);
    }

    [Fact]
    public async Task GetBoards_ShouldReturnListOfBoards()
    {
        var boards = await _service.GetBoards();
        Assert.NotEmpty(boards);
        Assert.IsType<List<ConwayBoard>>(boards);
    }

    [Fact]
    public async Task GetBoard_ShouldReturnBoardById()
    {
        var boardData = new List<List<bool>> 
        {
            new List<bool> { true, false, true },
            new List<bool> { false, true, false },
            new List<bool> { true, false, true }
        };
        var createdBoard = await _service.CreateBoard(new ConwayBoard(boardData, null));
        var retrievedBoard = await _service.GetBoard(createdBoard.Id.Value);

        Assert.Equal(createdBoard.Id, retrievedBoard.Id);
        Assert.Equal(boardData, retrievedBoard.Board);
    }

    [Fact]
    public async Task GetBoard_ShouldThrowException_WhenBoardNotFound()
    {
        await Assert.ThrowsAsync<Exception>(() => _service.GetBoard(Guid.NewGuid()));
    }

    [Fact]
    public async Task CreateBoard_ShouldInsertAndReturnBoard()
    {
        var boardData = new List<List<bool>> 
        {
            new List<bool> { true, false },
            new List<bool> { false, true }
        };
        var newBoard = new ConwayBoard(boardData, null);

        var createdBoard = await _service.CreateBoard(newBoard);

        Assert.NotNull(createdBoard.Id);
        Assert.Equal(boardData, createdBoard.Board);

        var retrievedBoard = await _service.GetBoard(createdBoard.Id.Value);
        Assert.Equal(createdBoard.Id, retrievedBoard.Id);
        Assert.Equal(boardData, retrievedBoard.Board);
    }

    [Fact]
    public async Task GetFutureState_ShouldReturnCorrectFutureState()
    {
        var boardData = new List<List<bool>> 
        {
            new List<bool> { false, true, false },
            new List<bool> { false, true, false },
            new List<bool> { false, true, false }
        };
        var board = await _service.CreateBoard(new ConwayBoard(boardData, null));

        var futureState = await _service.GetFutureState(board.Id.Value, 1);

        var expectedState = new List<List<bool>> 
        {
            new List<bool> { false, false, false },
            new List<bool> { true, true, true },
            new List<bool> { false, false, false }
        };

        Assert.Equal(expectedState, futureState);
    }

    [Fact]
    public async Task GetFinalState_ShouldReturnStableState()
    {
        var boardData = new List<List<bool>> 
        {
            new List<bool> { false, true, false },
            new List<bool> { false, true, false },
            new List<bool> { false, true, false }
        };
        var board = await _service.CreateBoard(new ConwayBoard(boardData, null));

        var finalState = await _service.GetFinalState(board.Id.Value, 10);

        var expectedState = new List<List<bool>> 
        {
            new List<bool> { false, true, false },
            new List<bool> { false, true, false },
            new List<bool> { false, true, false }
        };

        Assert.Equal(expectedState, finalState);
    }

    [Fact]
    public async Task GetFinalState_ShouldThrowException_IfNoStableStateWithinAttempts()
    {
        var boardData = new List<List<bool>> 
        {
            new List<bool> { true, true, false },
            new List<bool> { true, true, false },
            new List<bool> { false, false, false }
        };
        var board = await _service.CreateBoard(new ConwayBoard(boardData, null));
        await Assert.ThrowsAsync<Exception>(() => _service.GetFinalState(board.Id.Value, 1));
    }
}
