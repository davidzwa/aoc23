var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
    "input2.txt",
};
string filePath = files[0];
List<string> fileContent = File.ReadLines(filePath).ToList();

var times = new List<int>();
var distances = new List<int>();
foreach (var line in fileContent)
{
    if (line.Contains("Time"))
    {
        times = line.Replace("Time:", "")
            .Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
    }
    else if (line.Contains("Distance"))
    {
        distances = line.Replace("Distance:", "")
            .Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
    }
}

times.ForEach(Console.WriteLine);
distances.ForEach(Console.WriteLine);