using System.Data;
using System.Drawing;
using q11;

var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
};
string filePath = files[1];
List<string> fileContent = File.ReadLines(filePath).ToList();

int[,] map = new int[fileContent.Count, fileContent.First().Length];

var galaxy = '#';
var space = '.';
var emptyRows = new List<int>();
var emptyCols = new List<int>();
var size = new Size(fileContent.Count, fileContent.First().Length);
for (int y = 0; y < size.Height; y++)
{
    for (int x = 0; x < size.Width; x++)
    {
        map[y, x] = fileContent[y][x] == '#' ? 1 : 0;
    }

    if (Enumerable.Range(0, size.Width)
        .All(j => fileContent[y][j] == space))
    {
        emptyRows.Add(y);
    }

    // Col iterate
    if (Enumerable.Range(0, size.Height)
        .All(j => fileContent[j][y] == space))
    {
        emptyCols.Add(y);
    }
}

var offsetRowInsert = 0;
foreach (var rowIndex in emptyRows)
{
    map = Matrix.InsertRow(map, Enumerable.Range(0, size.Width).Select(v => 0).ToArray(), rowIndex + offsetRowInsert);
    size.Height++;
    offsetRowInsert++;
}

var offsetColumnInsert = 0;
foreach (var colIndex in emptyCols)
{
    map = Matrix.InsertColumn(map, Enumerable.Range(0, size.Height).Select(v => 0).ToArray(),
        colIndex + offsetColumnInsert);
    size.Width++;
    offsetColumnInsert++;
}

var locations = new List<(int X, int Y, int id)>();
for (int y = 0; y < size.Height; y++)
{
    for (int x = 0; x < size.Width; x++)
    {
        var isHash = map[y, x] == 1;
        if (isHash)
        {
            locations.Add((x, y, locations.Count + 1));
            Console.WriteLine(locations.Last());
        }
    }
}

Console.WriteLine($"Rows {string.Join(", ", emptyRows)}");
Console.WriteLine($"Cols {string.Join(", ", emptyCols)}");
Matrix.PrintArray(map);
Console.WriteLine(locations.Count);
Console.WriteLine(MathQ11.Binomial(locations.Count, 2));

var combinations = MathQ11.CombinationsOfK(locations.ToArray(), 2);
Console.WriteLine(combinations.Count());
var sumDistances = 0;
foreach (var comb in combinations)
{
    var valueTuples = comb as (int X, int Y, int id)[] ?? comb.ToArray();
    var c1 = valueTuples.ElementAt(0);
    var c2 = valueTuples.ElementAt(1);
    var diffX = Math.Abs(c2.X - c1.X);
    var diffY = Math.Abs(c2.Y - c1.Y);
    var dist = diffX + diffY;

    Console.Write(string.Join(" ", valueTuples.Select(c => string.Join(" ", c))));
    Console.WriteLine($"\t\t\tD={dist}");

    sumDistances += dist;
}

Console.WriteLine(sumDistances);