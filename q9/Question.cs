namespace q9;

public class Question
{
    public static List<int> ParseLine(string line)
    {
        return line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
        ;
    }

    public static (long, long) ProcessLinePart1(List<int> vals)
    {
        int oi = 0;

        var lookup = new List<List<int>> { GetRates(vals) };
        var lastRates = new List<int> { lookup.Last().Last() };
        while (lookup.Last().Any(c => c != 0))
        {
            if (oi > 1000)
            {
                throw new Exception("Too many loops");
            }

            lookup.Add(GetRates(lookup.Last()));
            lastRates.Add(lookup.Last().Last());
            oi++;
        }

        var firstRates = new List<int> { 0 };
        for (int i = lookup.Count - 2; i >= 0; i--)
        {
            var lst = lookup[i];
            var prevRate = firstRates[lookup.Count - 2 - i];
            // Console.Write($"{lst[0]} - {prevRate} = {lst[0] - prevRate}, ");
            firstRates.Add(lst[0] - prevRate);
        }

        // Console.WriteLine(firstRates.Sum(fr => fr));
        // Console.WriteLine(vals.First() - firstRates.Last());
        return (vals.First() - firstRates.Last(), vals.Last() + lastRates.Sum(l => (long)l));
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