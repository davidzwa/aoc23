using q12;

var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
};
string filePath = files[2];
List<string> fileContent = File.ReadLines(filePath).ToList();

List<Row> rows = new();
var sum = 0; 
foreach (var line in fileContent)
{
    var springRecord = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
    var springs = springRecord[0].ToList();
    var record = springRecord[1].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
    rows.Add(new()
    {
        Chars = springs,
        Record = record
    });

    // Console.Write(springs + " ");
    // Console.WriteLine(string.Join(", ", record));

    // if (rows.Count == 1)
    // {
        sum += Question.FindCombinations(springs, record);
    // }
}
Console.WriteLine($"Done {sum}");

class Row
{
    public List<char> Chars { get; set; }

    public List<int> Record { get; set; }
}