using q10.Utils;

internal static class Console
{
    public static void Log(ConsoleInterpolatedStringHandler builder)
    {
        builder.WriteLine();
    }

    public static void Write(ConsoleInterpolatedStringHandler builder)
    {
        builder.Write();
    }
}