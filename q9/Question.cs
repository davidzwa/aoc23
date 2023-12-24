namespace q9;

public class Question
{
    public static int ProcessLine(string line)
    {
        var vals = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
        var nextNumber = 0;

        int oi = 0;
        
        var copy = GetRates(vals);
        var lastRates = new List<int> { copy.Last() };
        while (copy.Any(c => c != 0))
        {
            if (oi > 1000)
            {
                throw new Exception("Too many loops");
            }
            
            copy = GetRates(copy);
            lastRates.Add(copy.Last());
            oi++;
        }
        
        // Console.WriteLine($"{oi}, {}");

        return vals.Last() + lastRates.Sum(l => l);
    }

    static List<int> GetRates(List<int> input)
    {
        List<int> rates = new(input.Count - 1);
        for (int i = 0; i < input.Count - 1; i++)
        {
            rates.Add(input[i + 1] - input[i]);
        }

        return rates;
    }
}