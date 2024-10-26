using System;
using Xunit;

public class ConwaysGameTests
{
    private readonly ConwaysGame game = new();

    /*
    HAPPY PATH
    Test for correct functionality
    */
    [Fact]
    public void UploadBoard_ShouldReturnUniqueBoardId()
    {
        bool[,] initialState = {
            { true, false, true },
            { false, true, false },
            { true, false, true }
        };

        var boardId = game.UploadBoard(initialState);
        Assert.NotEqual(Guid.Empty, boardId);
    }

    // Test a single tick
    [Fact]
    public void GetNextState_ShouldReturnCorrectNextState()
    {
        bool[,] initialState = {
            { false, true, false },
            { true, true, true },
            { false, true, false }
        };

        var boardId = game.UploadBoard(initialState);
        bool[,] expectedNextState = {
            { true, true, true },
            { true, false, true },
            { true, true, true }
        };

        var nextState = game.GetNextState(boardId);
        Assert.Equal(expectedNextState, nextState);
    }

    // Test multiple ticks
    [Fact]
    public void GetFutureState_ShouldReturnCorrectStateAfterXSteps()
    {
        bool[,] initialState = {
            { false, true, false },
            { true, true, true },
            { false, true, false }
        };

        var boardId = game.UploadBoard(initialState);

        bool[,] stateAfterTwoTicks = {
            { true, false, true },
            { false, false, false },
            { true, false, true }
        };

        var futureState = game.GetFutureState(boardId, 2);
        Assert.Equal(stateAfterTwoTicks, futureState);
    }

    // Get the final state
    [Fact]
    public void GetFinalState_ShouldReturnStableState()
    {
        bool[,] initialState = {
            { false, true, false },
            { true, true, true },
            { false, true, false }
        };

        var boardId = game.UploadBoard(initialState);
        bool[,] everybodyDies = {
            { false, false, false },
            { false, false, false },
            { false, false, false }
        };

        var finalState = game.GetFinalState(boardId, 10);
        Assert.Equal(everybodyDies, finalState);
    }

    /*
    UNHAPPY PATH
    Test for exceptions
    */
    [Fact]
    public void GetFinalState_ShouldThrowExceptionIfNoStableStateFound()
    {
        bool[,] initialState = {
            { true, false, true },
            { false, true, false },
            { true, false, true }
        };

        var boardId = game.UploadBoard(initialState);

        Exception ex = Assert.Throws<Exception>(() => game.GetFinalState(boardId, 2));
        Assert.Equal("Board did not reach a final state within the specified attempts.", ex.Message);
    }

    [Fact]
    public void GetNextState_ShouldThrowExceptionForInvalidBoardId()
    {
        var invalidBoardId = Guid.NewGuid();
        Exception ex = Assert.Throws<Exception>(() => game.GetNextState(invalidBoardId));
        Assert.Equal("Board not found.", ex.Message);
    }

    [Fact]
    public void GetFutureState_ShouldThrowExceptionForInvalidBoardId()
    {
        var invalidBoardId = Guid.NewGuid();
        Exception ex = Assert.Throws<Exception>(() => game.GetFutureState(invalidBoardId, 5));
        Assert.Equal("Board not found.", ex.Message);
    }

    [Fact]
    public void GetFinalState_ShouldThrowExceptionForInvalidBoardId()
    {
        var invalidBoardId = Guid.NewGuid();
        Exception ex = Assert.Throws<Exception>(() => game.GetFinalState(invalidBoardId, 10));
        Assert.Equal("Board not found.", ex.Message);
    }
}
