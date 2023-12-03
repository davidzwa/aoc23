string[] symbols = new[]
{
    "@",
    "#",
    "$",
    "%",
    "^",
    "&",
    "=",
    "*",
    "+",
    "-",
    "$",
    "/"
};


var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
};
string filePath = files[0];

List<string> fileContent = File.ReadLines(filePath).ToList();

var sumPartsPart1 = 0;
for (int row = 0; row < fileContent.Count; row++)
{
    var rowString = fileContent[row];
    bool foundNumber = false;
    string numberString = "";

    for (int col = 0; col < rowString.Length; col++)
    {
        var addedNumberNextCol = false;
        var conv = int.TryParse(rowString[col].ToString(), out int val);
        if (conv)
        {
            if (!foundNumber)
            {
                numberString = rowString[col].ToString();
            }
            else
            {
                numberString += rowString[col].ToString();
            }

            foundNumber = true;
        }
        else
        {
            if (foundNumber)
            {
                addedNumberNextCol = true;
            }

            foundNumber = false;
        }

        if (col == rowString.Length - 1 && foundNumber || addedNumberNextCol)
        {
            var numberLength = numberString.Length;
            var isPart = false;
            for (int searchRow = Math.Max(row - 1, 0);
                 searchRow <= Math.Min(fileContent.Count - 1, row + 1);
                 searchRow++)
            {
                var offset = addedNumberNextCol ? 1 : 0;
                var startIndex = Math.Max(col - numberLength - offset, 0);
                var endIndex = Math.Min(col + 1, fileContent[searchRow].Length);
                var substr = fileContent[searchRow].Substring(startIndex, endIndex - startIndex);
                Console.WriteLine(substr);
                isPart |= symbols.Any(s => substr.Contains(s));
            }

            Console.WriteLine($"Reporting number {numberString} isPart {isPart}");
            if (isPart)
            {
                sumPartsPart1 += int.Parse(numberString);
            }
        }
    }

    // foreach (var symbol in symbols)
    // {
    //     var originalRowString = fileContent[row];
    //     // When symbol found we need to look 'around it' for number symbols in the nearest cells
    //     if (originalRowString.Contains(symbol))
    //     {
    //         var col = originalRowString.IndexOf(symbol, StringComparison.InvariantCulture);
    //         Console.WriteLine($"({row},{col}) {symbol}");
    //
    //         
    //         for (int i = row-1; i <= Math.Min(fileContent.Count-1,row+1); i++)
    //         {
    //             var substr = fileContent[i].Substring(Math.Max(col - 3, 0),
    //                 Math.Min(7, fileContent[i].Length - col));
    //             Console.WriteLine(substr);
    //         }
    //         
    //         Console.WriteLine("");
    //         
    //         continue;
    //         
    //         // for (int compareCol = col - 1; compareCol <= col + 1; compareCol++)
    //         // {
    //         //     for (int compareRow = row-1; compareRow <= row+1; compareRow++)
    //         //     {
    //         //         var rowString = fileContent[compareRow];
    //         //         if (compareCol == col && compareRow == row)
    //         //         {
    //         //             if (!rowString[compareCol].Equals(char.Parse(symbol)))
    //         //             {
    //         //                 throw new Exception("Middle cell char not as expected");
    //         //             }
    //         //             // Console.WriteLine("Symbol found at " + compareRow + " " + compareCol + " " + symbol);
    //         //             continue;
    //         //         }
    //         //
    //         //         var i = int.TryParse(rowString[compareCol].ToString(), out int val);
    //         //         if (i)
    //         //         {
    //         //             var substr = rowString.Substring(Math.Max(compareCol - 2, 0),
    //         //                 Math.Min(4, rowString.Length - compareCol));
    //         //             // substr.Split(".");
    //         //             Console.WriteLine(substr);
    //         //             // places.Add((val, fileContent[compareRow].Length.ToString(), compareRow, compareCol));
    //         //             // Console.WriteLine((symbol, val, rowString.Length.ToString(), compareRow, compareCol));
    //         //             
    //         //         }
    //         //         Console.WriteLine("----");
    //         //     }
    //         // }
    //         
    //         
    //     }
    // }    
}

Console.WriteLine($"Part 1: {sumPartsPart1}");
// 546563 correct