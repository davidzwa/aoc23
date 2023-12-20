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

foreach (var line in fileContent)
{

}