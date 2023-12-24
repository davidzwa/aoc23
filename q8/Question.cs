namespace q8;

public class Question
{
    public static bool V2 = false;

    

    public static Direction ParseDirection(char dirChar)
    {
        return dirChar == 'L' ? Direction.Left : Direction.Right;
    }

    public static bool IsDone(string source)
    {
        return source == "ZZZ";
    }

    public static string ProcessLine((string Left, string Right) directions, Direction instruction)
    {
        if (instruction == Direction.Left)
        {
            return directions.Left;
        }

        return directions.Right;
    }
}