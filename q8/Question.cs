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
}