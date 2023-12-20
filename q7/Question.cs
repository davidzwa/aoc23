using System.Text.RegularExpressions;

namespace q7;

public class Question
{
    public static List<string> Chars =
        "A, K, Q, J, T, 9, 8, 7, 6, 5, 4, 3, 2".Split(", ", StringSplitOptions.RemoveEmptyEntries)
            .ToList();

    public static Dictionary<string, int> CharScores = Enumerable.Reverse(Chars)
        .Select((string c, int d) => new { c, d })
        .ToDictionary(c => c.c, c => c.d);


    public static int CompareHands((string Hand, long Bid, State State) hand1,
        (string Hand, long Bid, State State) hand2)
    {
        var largerThan = hand1.State.Type > hand2.State.Type;
        if (largerThan) return 1;
        if (hand1.State.Type == hand2.State.Type)
        {
            Console.WriteLine($"Equal {hand1.Hand} {hand1.State.Type} {hand2.Hand} {hand2.State.Type}");
            for (int i = 0; i < 5; i++)
            {
                var s1 = hand1.Hand[i].ToString();
                var s2 = hand2.Hand[i].ToString();
                if (CharScores[s1] > CharScores[s2])
                {
                    Console.WriteLine("Larger");
                    return 1;
                }

                if (CharScores[s1] < CharScores[s2])
                {
                    Console.WriteLine("Smaller");
                    return -1;
                }
            }

            throw new Exception("No tie break");
        }

        return -1;
    }

    public static void AnalyzeLines(ref List<(string Hand, long Bid, State State)> plays)
    {
        foreach (var play in plays)
        {
            play.State.Type = AnalyzeLine(play.Hand);
        }

        plays.Sort(CompareHands);

        long winnigs = 0;
        int cnt = 0;
        foreach (var play in plays)
        {
            cnt++;
            Console.WriteLine($"play {play.Hand} {play.Bid}*{cnt}={play.Bid * cnt}");
            winnigs += play.Bid * cnt;
        }
        Console.WriteLine($"Winnings {winnigs}");
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