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

    var direction = Question.ParseDirection(directions[j]);
    Console.WriteLine($"[{steps}] Source: {source.Key} Dirs: {source.Value} Next: {direction}");

    var targetName = Question.ProcessLine(source.Value, direction);
    source = new (targetName, map[targetName]);

    steps++;
    if (j == directions.Count - 1)
    {
        j = 0;
    }

    if (steps > 50)
    {
        throw new Exception("Ehm");
    }
}

Console.WriteLine($"Steps {steps}");
// 5041 too low