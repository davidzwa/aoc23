using System.Diagnostics;
using q8;

Question.V2 = false;
var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
};
string filePath = files[0];
List<string> fileContent = File.ReadLines(filePath).ToList();

Dictionary<string, (string Left, string Right)> map = new();
var directions = fileContent[0].Select(d => d).ToList();
foreach (var l in fileContent[2..])
{
    var line = l.Split(" = ");
    var lineLeftRight = line[1].Replace("(", "").Replace(")", "").Split(", ");
    map.Add(line[0], (Left: lineLeftRight[0], Right: lineLeftRight[1]));
}

var steps = 0;
var source = map.First(f => f.Key == "AAA");
for (int j = 0; j < directions.Count; j++)
{
    if (Question.IsDone(source.Key))
    {
        break;
    }

    source = ProcessNext(ref map, source.Value, directions[j]);
    Console.WriteLine($"[{steps}] Source: {source.Key} Dirs: {source.Value} Next: {source.Key}");

    steps++;
    if (j == directions.Count - 1)
    {
        j = -1;
    }
}

Console.WriteLine($"Steps {steps}");
// 5041 too low
// 19951 correct

KeyValuePair<string, (string Left, string Right)> ProcessNext(
    ref Dictionary<string, (string Left, string Right)> map,
    (string Left, string Right) options,
    char direction
)
{
    var targetName = Question.ProcessSourceDirection(direction, options);
    return new(targetName, map[targetName]);
}