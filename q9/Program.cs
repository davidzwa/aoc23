using q9;

var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
};
string filePath = files[0];
List<string> fileContent = File.ReadLines(filePath).ToList();

var (first,last) = (0L,0L);
var lines = fileContent.Select(Question.ParseLine).ToList();
foreach (var line in lines)
{
    var result = Question.ProcessLinePart1(line);

    Console.WriteLine(result);
    first += result.Item1;
    last += result.Item2;
}

Console.WriteLine($"{first} {last}");

// 1834108701 correct answer
// 993 correct