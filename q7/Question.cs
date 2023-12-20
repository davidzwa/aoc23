using System.Text.RegularExpressions;

namespace q7;

public class Question
{
    public static List<string> Chars =
        "A, K, Q, J, T, 9, 8, 7, 6, 5, 4, 3, 2".Split(", ", StringSplitOptions.RemoveEmptyEntries)
            .ToList();

    public static Dictionary<string, int> Scores = Enumerable.Reverse(Chars).Select((string c, int d) => new { c, d })
        .ToDictionary(c => c.c, c => c.d);

    public static void AnalyzeLines(ref List<(string Hand, long Bid, State State)> plays)
    {
        foreach (var play in plays)
        {
            play.State.Type = AnalyzeLine(play.Hand);
            Console.WriteLine($"Line {play.Hand} Type {play.State.Type}");
        }

        // Console.WriteLine(plays.First().State.Type);
    }

    public static HandType AnalyzeLine(string line)
    {
        var dict = new Dictionary<string, int>();
        foreach (var c in Chars)
        {
            var count = Regex.Count(line, c);
            dict[c] = count;
        }

        var max = dict.Max(d => d.Value);
        if (max == 5)
        {
            return HandType.FiveOfAKind;
        }

        if (max == 4)
        {
            return HandType.FourOfAKind;
        }

        if (dict.Any(d => d.Value == 3) && dict.Any(d => d.Value == 2))
        {
            return HandType.FullHouse;
        }

        if (max == 3)
        {
            return HandType.ThreeOfAKind;
        }

        if (max == 2 && dict.Count(v => v.Value == 2) == 2)
        {
            return HandType.TwoPair;
        }
        
        if (max == 2 && dict.Count(v => v.Value == 2) == 1)
        {
            return HandType.OnePair;
        }

        return HandType.HighCard;
    }
}