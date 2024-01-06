var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
};
string filePath = files[1];
List<string> fileContent = File.ReadLines(filePath).ToList();

List<Row> rows = new();
foreach (var line in fileContent)
{
    var springRecord = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
    var springs = springRecord[0];
    var record = springRecord[1].Split(",", StringSplitOptions.RemoveEmptyEntries);
    rows.Add(new ()
    {
        Chars = springs.ToList(),
        Record = record.Select(int.Parse).ToList()
    });
    
    Console.Write(springs + " ");
    Console.WriteLine(string.Join(", ", record));
}

class Row
{
    public List<char> Chars { get; set; }
    
    public List<int> Record { get; set; }
    
}