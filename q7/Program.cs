﻿using q7;

var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
};
string filePath = files[0];
List<string> fileContent = File.ReadLines(filePath).ToList();

// Console.WriteLine(dict);

List<(string Hand, long Bid, State state)> plays = new();
foreach (var line in fileContent)
{
    var split = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
    plays.Add((split[0], long.Parse(split[1]), new ()));
}

Question.AnalyzeLines(ref plays);
// 251216224 correct

Console.WriteLine("Part 2");
Question.V2 = true;
Question.AnalyzeLines(ref plays);

public class State
{
    public int JCount { get; set; }
    public HandType Type { get; set; }
}