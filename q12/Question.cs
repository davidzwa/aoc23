namespace q12;

public class Question
{
    public static int FindCombinations(List<char> springs, List<int> record)
    {
        var groups = record.Count;
        var accounted = record.Sum();
        var separators = Math.Max(groups - 1, 0);
        var empty = springs.Count(s => s == '.');
        var unresolved = springs.Count(s => s == '?');
        
        Console.WriteLine($"A={accounted} S={separators} U={unresolved}");
        
        Console.WriteLine(string.Join("", springs));
        Console.WriteLine(string.Join(" ", record));

        // Looking for combinations of how groups can fit in the unknown sections
        return 1;
    }
}