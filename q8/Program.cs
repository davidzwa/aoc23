using q8;

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
var isV2 = true;
var sources = FindStartingPoints(ref map, isV2);
for (int j = 0; j < directions.Count; j++)
{
    if (isV2 && Question.IsDoneV2(sources.Select(s => s.Key).ToList()))
    {
        Console.WriteLine("Done V2");
        break;
    }

    if (!isV2 && Question.IsDone(sources.First().Key))
    {
        Console.WriteLine("Done V1");
        break;
    }

    var newSources = new Dictionary<string, (string Left, string Right)>();
    foreach (var source in sources)
    {
        var sourceUpdate = ProcessNext(ref map, source.Value, directions[j]);
        newSources.Add(sourceUpdate.Key, sourceUpdate.Value);
        Console.WriteLine($"[{steps}] Source: {source.Key} Dirs: {source.Value} Next: {source.Key}");

    }

    steps++;
    Console.WriteLine("\nNext round");

    if (j == directions.Count - 1)
    {
        j = -1;
    }

    sources = newSources;
}

Console.WriteLine($"Steps {steps}");
// 5041 too low
// 19951 correct

Dictionary<string, (string Left, string Right)> FindStartingPoints(
    ref Dictionary<string, (string Left, string Right)> map, bool isV2)
{
    if (!isV2)
    {
        return map.Where(f => f.Key == "AAA").ToDictionary();
    }

    return map.Where(m => m.Key.EndsWith('A')).ToDictionary();
}

KeyValuePair<string, (string Left, string Right)> ProcessNext(
    ref Dictionary<string, (string Left, string Right)> map,
    (string Left, string Right) options,
    char direction
)
{
    var targetName = Question.ProcessSourceDirection(direction, options);
    return new(targetName, map[targetName]);
}