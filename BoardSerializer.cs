using System;
using System.Collections.Generic;
using System.Text.Json;

public static class BoardSerializer
{
    // Convert bool[,] to List<List<bool>> for serialization
    public static List<List<bool>> ToSerializableFormat(bool[,] array)
    {
        var list = new List<List<bool>>();
        for (int i = 0; i < array.GetLength(0); i++)
        {
            var row = new List<bool>();
            for (int j = 0; j < array.GetLength(1); j++)
            {
                row.Add(array[i, j]);
            }
            list.Add(row);
        }
        return list;
    }

    // Convert List<List<bool>> back to bool[,]
    public static bool[,] FromSerializableFormat(List<List<bool>> list)
    {
        int rows = list.Count;
        int cols = list[0].Count;
        var array = new bool[rows, cols];
        
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                array[i, j] = list[i][j];
            }
        }
        return array;
    }

    // Serialize the bool[,] array to a JSON string
    public static string SerializeBoard(bool[,] board)
    {
        var serializableBoard = ToSerializableFormat(board);
        return JsonSerializer.Serialize(serializableBoard);
    }

    // Deserialize the JSON string back to a bool[,] array
    public static bool[,] DeserializeBoard(string json)
    {
        var deserializedList = JsonSerializer.Deserialize<List<List<bool>>>(json);
        return FromSerializableFormat(deserializedList);
    }
}
