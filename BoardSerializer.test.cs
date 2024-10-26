using System;
using System.Collections.Generic;
using Xunit;

public class BoardSerializerTests
{
    [Fact]
    public void ToSerializableFormat_ShouldConvert2DArrayToListOfLists()
    {
        bool[,] board = {
            { true, false, true },
            { false, true, false },
            { true, false, true }
        };

        var expected = new List<List<bool>>
        {
            new List<bool> { true, false, true },
            new List<bool> { false, true, false },
            new List<bool> { true, false, true }
        };

        var result = BoardSerializer.ToSerializableFormat(board);

        Assert.Equal(expected.Count, result.Count);
        for (int i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i], result[i]);
        }
    }

    [Fact]
    public void FromSerializableFormat_ShouldConvertListOfListsTo2DArray()
    {
        var list = new List<List<bool>>
        {
            new List<bool> { true, false, true },
            new List<bool> { false, true, false },
            new List<bool> { true, false, true }
        };

        bool[,] expected = {
            { true, false, true },
            { false, true, false },
            { true, false, true }
        };

        var result = BoardSerializer.FromSerializableFormat(list);

        Assert.Equal(expected.GetLength(0), result.GetLength(0));
        Assert.Equal(expected.GetLength(1), result.GetLength(1));

        for (int i = 0; i < expected.GetLength(0); i++)
        {
            for (int j = 0; j < expected.GetLength(1); j++)
            {
                Assert.Equal(expected[i, j], result[i, j]);
            }
        }
    }

    [Fact]
    public void SerializeBoard_ShouldReturnCorrectJsonString()
    {
        bool[,] board = {
            { true, false, true },
            { false, true, false },
            { true, false, true }
        };

        string expectedJson = "[[true,false,true],[false,true,false],[true,false,true]]";
        
        string json = BoardSerializer.SerializeBoard(board);

        Assert.Equal(expectedJson, json);
    }

    [Fact]
    public void DeserializeBoard_ShouldReturnOriginal2DArray()
    {
        string json = "[[true,false,true],[false,true,false],[true,false,true]]";

        bool[,] expected = {
            { true, false, true },
            { false, true, false },
            { true, false, true }
        };

        bool[,] result = BoardSerializer.DeserializeBoard(json);

        Assert.Equal(expected.GetLength(0), result.GetLength(0));
        Assert.Equal(expected.GetLength(1), result.GetLength(1));

        for (int i = 0; i < expected.GetLength(0); i++)
        {
            for (int j = 0; j < expected.GetLength(1); j++)
            {
                Assert.Equal(expected[i, j], result[i, j]);
            }
        }
    }

    [Fact]
    public void SerializeAndDeserialize_ShouldReturnEquivalent2DArray()
    {
        bool[,] board = {
            { true, false, true },
            { false, true, false },
            { true, false, true }
        };

        string json = BoardSerializer.SerializeBoard(board);
        bool[,] deserializedBoard = BoardSerializer.DeserializeBoard(json);

        Assert.Equal(board.GetLength(0), deserializedBoard.GetLength(0));
        Assert.Equal(board.GetLength(1), deserializedBoard.GetLength(1));

        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                Assert.Equal(board[i, j], deserializedBoard[i, j]);
            }
        }
    }
}
