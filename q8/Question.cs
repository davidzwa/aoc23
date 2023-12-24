using System.Numerics;

namespace q8;

public class Question
{
    public static string ProcessSourceDirection(char directionStr, (string Left, string Right) options)
    {
        var direction = ParseDirection(directionStr);
        return ProcessDirection(options, direction);
    }

    public static string ProcessDirection((string Left, string Right) directions, Direction instruction)
    {
        if (instruction == Direction.Left)
        {
            return directions.Left;
        }

        return directions.Right;
    }

    public static Direction ParseDirection(char dirChar)
    {
        return dirChar == 'L' ? Direction.Left : Direction.Right;
    }

    public static bool IsDoneV2(List<string> sources)
    {
        return sources.All(s => s.EndsWith('Z'));
    }

    public static bool IsDone(string source)
    {
        return source == "ZZZ";
    }

    public static void PrimeFactors(long n)
    {
        while (n % 2 == 0)
        {
            // Console.Write(2 + " ");
            n /= 2;
        }

        for (int i = 3; i <= Math.Sqrt(n); i += 2)
        {
            while (n % i == 0)
            {
                // Console.Write(i + " ");
                n /= i;
            }
        }

        if (n > 2)
        // output factors
        Console.WriteLine(n);
    }
}