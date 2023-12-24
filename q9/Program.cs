using q9;

var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
};
string filePath = files[0];
List<string> fileContent = File.ReadLines(filePath).ToList();

var sum = 0;
foreach (var line in fileContent)
{
    sum += Question.ProcessLine(line);
}

Console.WriteLine(sum);

// 1834108701 correct answer