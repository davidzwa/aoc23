namespace q11;

using System;

class Matrix
{
    // static void Main()
    // {
    //     int[,] originalArray = {
    //         {1, 2, 3},
    //         {4, 5, 6},
    //         {7, 8, 9}
    //     };
    //
    //     Console.WriteLine("Original Array:");
    //     PrintArray(originalArray);
    //
    //     // Insert a new row at index 1
    //     int[,] newArrayWithRow = InsertRow(originalArray, new int[] { 10, 11, 12 }, 1);
    //     Console.WriteLine("\nArray after inserting a row:");
    //     PrintArray(newArrayWithRow);
    //
    //     // Insert a new column at index 2
    //     int[,] newArrayWithColumn = InsertColumn(originalArray, new int[] { 30, 60, 90 }, 2);
    //     Console.WriteLine("\nArray after inserting a column:");
    //     PrintArray(newArrayWithColumn);
    // }

    public static int[,] InsertRow(int[,] array, int[] newRow, int rowIndex)
    {
        int rows = array.GetLength(0);
        int columns = array.GetLength(1);

        int[,] newArray = new int[rows + 1, columns];

        for (int i = 0; i < rowIndex; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                newArray[i, j] = array[i, j];
            }
        }

        for (int j = 0; j < columns; j++)
        {
            newArray[rowIndex, j] = newRow[j];
        }

        for (int i = rowIndex + 1; i < rows + 1; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                newArray[i, j] = array[i - 1, j];
            }
        }

        return newArray;
    }

    public static int[,] InsertColumn(int[,] array, int[] newColumn, int columnIndex)
    {
        int rows = array.GetLength(0);
        int columns = array.GetLength(1);

        int[,] newArray = new int[rows, columns + 1];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columnIndex; j++)
            {
                newArray[i, j] = array[i, j];
            }

            newArray[i, columnIndex] = newColumn[i];

            for (int j = columnIndex + 1; j < columns + 1; j++)
            {
                newArray[i, j] = array[i, j - 1];
            }
        }

        return newArray;
    }

    public static void PrintArray(int[,] array)
    {
        int rows = array.GetLength(0);
        int columns = array.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                var repr = array[i, j] == 1 ? "x" : ".";
                Console.Write(repr + "");
            }
            Console.WriteLine();
        }
    }
}
