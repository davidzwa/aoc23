// See https://aka.ms/new-console-template for more information

// Read a file

var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
};
string filePath = files[0];
List<string> fileContent = File.ReadLines(filePath).ToList();
(int Red, int Green, int Blue) limits = (12, 13, 14);

var gameLines = new List<Line>();
var possibleGames = new List<Line>();
foreach (var line in fileContent)
{
    if (line.Contains("Out: "))
    {
        Console.WriteLine(line);
        continue;
    }

    var substr = line.Substring(5);
    var game = substr.Replace(" ", "");
    var splits = game.Split(":", 2);
    var gameIndex = int.Parse(splits.First());

    var allRounds = splits[1].Split(";");
    // 12 red, 2 green, 5 blue;
    // 12red,2green,5blue;
    var rounds = allRounds.Select(ar =>
    {
        var rgbSplit = ar.Split(",");

        (int Red, int Green, int Blue) round = new();
        foreach (var rgbOption in rgbSplit)
        {
            var spl = rgbOption.Split("red");
            if (spl.Length > 1)
            {
                round.Red = int.Parse(spl[0]);
            }

            var spl2 = rgbOption.Split("green");
            if (spl2.Length > 1)
            {
                round.Green = int.Parse(spl2[0]);
            }

            var spl3 = rgbOption.Split("blue");
            if (spl3.Length > 1)
            {
                round.Blue = int.Parse(spl3[0]);
            }
        }

        return round;
    });

    var maxRound = rounds.Aggregate((a, b) =>
        (Red: Math.Max(a.Red, b.Red), Green: Math.Max(a.Green, b.Green), Blue: Math.Max(a.Blue, b.Blue)));

    var gameIsPossible = maxRound.Red <= limits.Red && maxRound.Green <= limits.Green && maxRound.Blue <= limits.Blue;
    gameLines.Add(new Line
    {
        GameIsPossible = gameIsPossible,
        Index = gameIndex,
        OriginalLine = line,
        Game = game,
        MaxOfRounds = maxRound,
        Rounds = rounds.ToList()
    });

    if (gameIsPossible)
    {
        possibleGames.Add(new Line
        {
            GameIsPossible = gameIsPossible,
            Index = gameIndex,
            OriginalLine = line,
            Game = game,
            MaxOfRounds = maxRound,
            Rounds = rounds.ToList()
        });
    }

    Console.WriteLine(gameIndex + " " + maxRound + " " + gameIsPossible);
}

var sumIndices = possibleGames.Sum(pg => pg.Index);
Console.WriteLine("Sum indices: " + sumIndices);
// 2600 correct

var powers = gameLines.Sum(r => r.MaxOfRounds.Red * r.MaxOfRounds.Green * r.MaxOfRounds.Blue);
Console.WriteLine("Sum powers: " + powers);

class Line
{
    public bool GameIsPossible { get; set; }
    public int Index { get; set; }

    public string OriginalLine { get; set; }

    public string Game { get; set; }

    public List<(int Red, int Green, int Blue)> Rounds { get; set; }

    public (int Red, int Green, int Blue) MaxOfRounds { get; set; }
}