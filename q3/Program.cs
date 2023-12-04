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

// For part two we are first going to find any gears '*' and put their location in a X-Y-countdict
Dictionary<(int, int), (int, int)> dict = new Dictionary<(int, int), (int, int)>();

for (int row = 0; row < fileContent.Count; row++)
{
    foreach (var symbol in symbols)
    {
        var originalRowString = fileContent[row];
        if (originalRowString.Contains(symbol))
        {
            var col = originalRowString.IndexOf(symbol, StringComparison.InvariantCulture);
            dict[(row, col)] = (0,1);
        }
    }
}

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
            var converted = int.Parse(numberString);
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
                var gearAt = substr.IndexOf('*');
                if (gearAt != -1)
                {
                    Console.WriteLine($"Gear! {substr[gearAt]} ({searchRow},{startIndex + gearAt})");
                    if (!dict.ContainsKey((searchRow, startIndex + gearAt)))
                    {
                        dict.Add((searchRow, startIndex + gearAt), (1, converted));
                    }
                    else
                    {
                        var dictVal = dict[(searchRow, startIndex + gearAt)];
                        dictVal.Item1++;
                        dictVal.Item2 *= converted;
                        dict[(searchRow, startIndex + gearAt)] = dictVal;
                    }
                }
            }

            Console.WriteLine($"Reporting number {numberString} isPart {isPart}");
            if (isPart)
            {
                sumPartsPart1 += converted;
            }
        }
    }

    // Here I had code to iterate over symbols with a search. Not super practical compared to the hitbox-approach above.
}

Console.WriteLine($"Part 1: {sumPartsPart1}");
// 546563 correct

Console.WriteLine(dict.Where((k, v) => k.Value.Item1 == 2).Select(k => k.Value.Item2).Sum());