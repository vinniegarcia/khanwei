using System;
using System.Collections.Generic;
using Xunit;

public class ConwaysGameMoveTests
{
    [Fact]
    public void GetNextState_ShouldReturnCorrectNextState()
    {
        bool[,] initialState = {
            { false, true, false },
            { false, true, false },
            { false, true, false }
        };

        var initialBoard = new ConwayBoard(BoardSerializer.ToSerializableFormat(initialState), Guid.NewGuid());
        var expectedNextState = new List<List<bool>> {
            new List<bool> { false, false, false },
            new List<bool> { true,  true,  true  },
            new List<bool> { false, false, false }
        };

        var nextState = ConwaysGameMove.GetNextState(initialBoard);

        Assert.Equal(expectedNextState, nextState);
    }

    [Fact]
    public void GetFutureState_ShouldReturnCorrectStateAfterMultipleSteps()
    {
        bool[,] initialState = {
            { false, true, false },
            { false, true, false },
            { false, true, false }
        };

        var initialBoard = new ConwayBoard(BoardSerializer.ToSerializableFormat(initialState), Guid.NewGuid());
        var expectedFutureState = new List<List<bool>> {
            new List<bool> { false, true, false },
            new List<bool> { false, true, false },
            new List<bool> { false, true, false }
        };

        var futureState = ConwaysGameMove.GetFutureState(initialBoard, 2);
        Assert.Equal(expectedFutureState, futureState);
    }

    [Fact]
    public void GetFinalState_ShouldReturnFinalStableState()
    {
        bool[,] initialState = {
            { false, true, false },
            { false, true, false },
            { false, true, false }
        };

        var initialBoard = new ConwayBoard(BoardSerializer.ToSerializableFormat(initialState), Guid.NewGuid());
        var expectedFinalState = new List<List<bool>> {
            new List<bool> { false, true, false },
            new List<bool> { false, true, false },
            new List<bool> { false, true, false }
        };

        var finalState = ConwaysGameMove.GetFinalState(initialBoard, 10);
        Assert.Equal(expectedFinalState, finalState);
    }

    [Fact]
    public void GetFinalState_ShouldThrowException_IfNoFinalStateWithinAttempts()
    {
        bool[,] initialState = {
            { true, true, false },
            { true, true, false },
            { false, false, false }
        };

        var initialBoard = new ConwayBoard(BoardSerializer.ToSerializableFormat(initialState), Guid.NewGuid());

        Assert.Throws<Exception>(() => ConwaysGameMove.GetFinalState(initialBoard, 1));
    }

}
