namespace q7;

public class Question
{
    public static void AnalyzeLines(ref List<(string Hand, long Bid, State State)> plays)
    {
        foreach (var play in plays)
        {
            play.State.Type = AnalyzeLine(play.Hand);
        }
        Console.WriteLine(plays.First().State.Type);
    }
    
    public static  HandType AnalyzeLine(string line)
    {
        
        return HandType.FullHouse;
    }
}