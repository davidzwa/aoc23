using q6;

var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
    "input2.txt",
};
string filePath = files[1];
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

var races = new List<(int Time, int Distance, State state)>();
for (int i = 0; i < times.Count; i++)
{
    races.Add((times[i], distances[i], new()));
}

var result = 1l;
foreach (var race in races)
{
    result *= Question.ProcessRace(race);
}

Console.WriteLine("Result 1" + result);

string timeString = "";
string distanceString = "";
foreach (var td in races)
{
    timeString += td.Time.ToString();
    distanceString += td.Distance.ToString();
}

Console.WriteLine("Time " + timeString);
Console.WriteLine("Distance " + distanceString);
(long Time, long Distance, State state) raceB = new(long.Parse(timeString), long.Parse(distanceString), new());
var options = Question.ProcessRace(raceB);

// 235150181 too high
Console.WriteLine("Result 2 " + options);