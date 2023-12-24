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


var isV2 = true;

// brute force method does not finish
// for (int j = 0; j < directions.Count; j++)
// {
//     if (Question.IsDoneV2(sources.Select(s => s.Key).ToList()))
//     {
//         Console.WriteLine("Done V2");
//         break;
//     }
//
//     // if (!isV2 && Question.IsDone(sources.First().Key))
//     // {
//     //     Console.WriteLine("Done V1");
//     //     break;
//     // }
//
//     var newSources = new Dictionary<string, (string Left, string Right)>();
//     foreach (var source in sources)
//     {
//         var sourceUpdate = ProcessNext(ref map, source.Value, directions[j]);
//         newSources.Add(sourceUpdate.Key, sourceUpdate.Value);
//         // Console.WriteLine($"[{steps}] Source: {source.Key} {directions[j]} Dirs: {source.Value} Next: {sourceUpdate.Key}");
//     }
//
//     // var enders = sources.Where(s => s.Key.EndsWith('Z')).Count();
//
//     // if (enders > 4)
//     // {
//     //     Console.WriteLine($"E: {enders}");
//     // }
//     if (steps % 1000000 == 0)
//     {
//         Console.WriteLine($"R{steps:n0}");
//     }
//     
//     steps++;
//     // Console.WriteLine("\nNext round");
//
//     if (j == directions.Count - 1)
//     {
//         j = -1;
//     }
//
//     // if (steps == 4)
//     // {
//     //     return;
//     // }
//
//     sources = newSources;
// }

if (!isV2)
{
    var stepsV1 = ProcessV1(ref directions);
    Console.WriteLine($"Steps {stepsV1}");
}
else
{
    var sources = FindStartingPoints(ref map, isV2);
    Console.WriteLine("Sources");
    foreach (var keyValuePair in sources)
    {
        // Console.WriteLine(keyValuePair);
        var (cyclePeriod, end) = FindCycle(ref directions, keyValuePair.Key);
        // Console.WriteLine($"{keyValuePair.Key} Enters+Cycles in {cyclePeriod}");
        var (cyclePeriod2, _) = FindCycle(ref directions, end);
        if (cyclePeriod != cyclePeriod2)
        {
            throw new Exception("Cycle test outcomes not equal");
        }
        Console.WriteLine($"{keyValuePair.Key} to {end} in {cyclePeriod2}");

        Question.PrimeFactors(cyclePeriod2);
    }
}

// 5041 too low
// 19951 correct

// now get the LCM:
// 16,342,438,708,751 correct

(long, string) FindCycle(ref List<char> dirs, string start)
{
    // var startDirection = dirs[0];
    var steps = 0;
    var sourceCycle = map.First(m => m.Key == start);
    
    for (int j = 0; j < dirs.Count; j++)
    {
        var currentDirection = directions[j];
        if (steps != 0 && sourceCycle.Key.EndsWith("Z"))
        {
            // Console.WriteLine($"Found cycle {start} {startDirection} {currentDirection} in {steps} steps");
            break;
        }

        var sourceUpdate = ProcessNext(ref map, sourceCycle.Value, currentDirection);
        sourceCycle = new KeyValuePair<string, (string Left, string Right)>(sourceUpdate.Key, sourceUpdate.Value);
        // Console.WriteLine(
        //     $"[{steps}] Source: {sourceCycle.Key} {directions[j]} Dirs: {sourceCycle.Value} Next: {sourceUpdate.Key}");

        // if (steps % 1000000 == 0)
        // {
        //     Console.WriteLine($"R{steps:n0}");
        // }

        steps++;

        if (j == dirs.Count - 1)
        {
            j = -1;
        }
    }

    return (steps,sourceCycle.Key);
}

long ProcessV1(ref List<char> dirs)
{
    var steps = 0l;
    var sourceV1 = FindStartingPoints(ref map, false).First();
    for (int j = 0; j < dirs.Count; j++)
    {
        if (Question.IsDone(sourceV1.Key))
        {
            Console.WriteLine("Done V1");
            break;
        }

        var sourceUpdate = ProcessNext(ref map, sourceV1.Value, directions[j]);
        sourceV1 = new KeyValuePair<string, (string Left, string Right)>(sourceUpdate.Key, sourceUpdate.Value);
        // Console.WriteLine(
        //     $"[{steps}] Source: {sourceV1.Key} {directions[j]} Dirs: {sourceV1.Value} Next: {sourceUpdate.Key}");

        if (steps % 1000000 == 0)
        {
            Console.WriteLine($"R{steps:n0}");
        }

        steps++;

        if (j == dirs.Count - 1)
        {
            j = -1;
        }
    }

    return steps;
}

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