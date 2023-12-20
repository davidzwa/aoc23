using q7;

var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
    "input2.txt",
};
string filePath = files[1];
List<string> fileContent = File.ReadLines(filePath).ToList();

var chars =
    "A, K, Q, J, T, 9, 8, 7, 6, 5, 4, 3, 2".Split(", ", StringSplitOptions.RemoveEmptyEntries)
        .ToList();

var dict = Enumerable.Reverse(chars).Select((string c, int d) => new { c, d }).ToDictionary(c => c.c, c => c.d);
// Console.WriteLine(dict);

List<(string Hand, long Bid, State state)> plays = new();
foreach (var line in fileContent)
{
    var split = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
    plays.Add((split[0], long.Parse(split[1]), new ()));
}

Question.AnalyzeLines(ref plays);

public class State
{
    public HandType Type { get; set; }
}