using System.Text.RegularExpressions;

namespace q7;

public class Question
{
    public static bool V2 = false;

    public static List<string> Chars =
        "A, K, Q, J, T, 9, 8, 7, 6, 5, 4, 3, 2".Split(", ", StringSplitOptions.RemoveEmptyEntries)
            .ToList();

    public static List<string> CharsV2 = "A, K, Q, T, 9, 8, 7, 6, 5, 4, 3, 2, J"
        .Split(", ", StringSplitOptions.RemoveEmptyEntries)
        .ToList();

    public static Dictionary<string, int> CharScores = Enumerable.Reverse(Chars)
        .Select((string c, int d) => new { c, d })
        .ToDictionary(c => c.c, c => c.d);

    public static Dictionary<string, int> CharScoresV2 = Enumerable.Reverse(CharsV2)
        .Select((string c, int d) => new { c, d })
        .ToDictionary(c => c.c, c => c.d);

    public static int CompareHands((string Hand, long Bid, State State) hand1,
        (string Hand, long Bid, State State) hand2)
    {
        var scores = V2 ? CharScoresV2 : CharScores;
        var largerThan = hand1.State.Type > hand2.State.Type;
        if (largerThan) return 1;
        if (hand1.State.Type == hand2.State.Type)
        {
            // Console.WriteLine($"Equal {hand1.Hand} {hand1.State.Type} {hand2.Hand} {hand2.State.Type}");
            for (int i = 0; i < 5; i++)
            {
                var s1 = hand1.Hand[i].ToString();
                var s2 = hand2.Hand[i].ToString();
                if (scores[s1] > scores[s2])
                {
                    return 1;
                }

                if (scores[s1] < scores[s2])
                {
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
            var result = AnalyzeLine(play.Hand);

            play.State.Type = result.Type;
            play.State.JCount = result.JCount;
            if (V2)
            {
                Console.WriteLine($"Play {play.Hand} Type {play.State.Type} JCount {play.State.JCount}");
            }
        }

        plays.Sort(CompareHands);

        long winnigs = 0;
        int cnt = 0;
        foreach (var play in plays)
        {
            cnt++;
            // Console.WriteLine($"play {play.Hand} {play.Bid}*{cnt}={play.Bid * cnt}");
            winnigs += play.Bid * cnt;
        }

        Console.WriteLine($"Winnings {winnigs}");
    }

    public static (HandType Type, int JCount) AnalyzeLine(string line)
    {
        var dict = new Dictionary<string, int>();
        foreach (var c in Chars)
        {
            var count = Regex.Count(line, c);
            dict[c] = count;
        }

        var jCount = V2 ? dict["J"] : 0;
        var max = dict.Max(d => d.Key != "J" || !V2 ? d.Value : 0);
        if (max == 0 && jCount < 5)
        {
            throw new Exception("Shouldnt happen");
        }
        if (max + jCount >= 5)
        {
            return (HandType.FiveOfAKind, jCount);
        }

        if (max + jCount > 4)
        {
            throw new Exception("Cant happen");
        }

        if (max + jCount == 4)
        {
            return (HandType.FourOfAKind, jCount);
        }

        if (!V2 && dict.Any(d => d.Value == 3) && dict.Any(d => d.Value == 2))
        {
            return (HandType.FullHouse, 0);
        }
        
        var max2Cnt = dict.Count(d => d.Value == 2);
        // J: 2,1,0
        if (max == 3 && dict.Any(d => d.Value + jCount == 2) ||
            max == 2 && max2Cnt == 2 && dict.Any(d => d.Value + jCount == 3))
        {
            return (HandType.FullHouse, jCount);
        }

        // J: 2,1,0
        if (max + jCount == 3)
        {
            return (HandType.ThreeOfAKind, jCount);
        }

        if (jCount == 2)
        {
            throw new Exception("Cannot happen");
        }

        // jcount is 1 or 0
        if (max == 2 && (dict.Count(v => v.Value == 2) == 2 || jCount == 1))
        {
            return (HandType.TwoPair, jCount);
        }

        if (jCount == 1) return (HandType.OnePair, jCount);

        if (max == 2 && dict.Count(v => v.Value == 2) == 1)
        {
            return (HandType.OnePair, jCount);
        }

        return (HandType.HighCard, jCount);
    }
}