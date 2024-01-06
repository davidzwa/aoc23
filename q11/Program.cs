using System.Drawing;
using q11;

var files = new[]
{
    "input.txt",
    "example.txt"
    // "test1.txt",
};
string filePath = files[0];
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
// Matrix.PrintArray(map);
Console.WriteLine($"Locations {locations.Count}");
// Console.WriteLine(MathQ11.Binomial(locations.Count, 2));

var combinations = MathQ11.CombinationsOfK(locations.ToArray(), 2);
var enumerable = combinations as IEnumerable<(int X, int Y, int id)>[] ?? combinations.ToArray();
Console.WriteLine($"Combinations {enumerable.Count()}");
var sumDistancesPart1 = 0;
var sumDistancesPart2 = 0L;
var spaceInflatePart1 = 2;
long spaceInflatePart2 = 1_000_000;
foreach (var comb in enumerable)
{
    var valueTuples = comb as (int X, int Y, int id)[] ?? comb.ToArray();
    var c1 = valueTuples.ElementAt(0);
    var c2 = valueTuples.ElementAt(1);

    var minX = Math.Min(c1.X, c2.X);
    var maxX = Math.Max(c1.X, c2.X);
    var minY = Math.Min(c1.Y, c2.Y);
    var maxY = Math.Max(c1.Y, c2.Y);

    var count1 = emptyRows.Where(r => r > minY && r < maxY).ToList().Count;
    var count2 = emptyCols.Where(r => r > minX && r < maxX).ToList().Count;

    var diffX = Math.Abs(c2.X - c1.X);
    var diffY = Math.Abs(c2.Y - c1.Y);
    var dist = diffX + diffY;

    // Console.Write(string.Join(" ", valueTuples.Select(c => string.Join(" ", c))));
    // Console.WriteLine($"\t\t\tD={dist}");

    sumDistancesPart2 += (long)dist + (long)(spaceInflatePart2 - 1) * count1 + (long)(spaceInflatePart2 - 1) * count2;
    sumDistancesPart1 += dist + (spaceInflatePart1 - 1) * count1 + (spaceInflatePart1 - 1) * count2;
}

// 374 example correct
// 9639160 correct
Console.WriteLine(sumDistancesPart1);
Console.WriteLine(sumDistancesPart2);
// 752936133304 correct