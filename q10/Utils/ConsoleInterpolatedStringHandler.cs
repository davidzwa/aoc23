﻿using System.Runtime.CompilerServices;

namespace q10.Utils;

[InterpolatedStringHandler]
public ref struct ConsoleInterpolatedStringHandler
{
    private static readonly Dictionary<string, ConsoleColor> colors;
    private readonly IList<Action> actions;

    static ConsoleInterpolatedStringHandler() =>
        colors = Enum.GetValues<ConsoleColor>().ToDictionary(x => x.ToString().ToLowerInvariant(), x => x);

    public ConsoleInterpolatedStringHandler(int literalLength, int formattedCount)
    {
        actions = new List<Action>();
    }

    public void AppendLiteral(string s)
    {
        actions.Add(() => System.Console.Write(s));
    }

    public void AppendFormatted<T>(T t)
    {
        actions.Add(() => System.Console.Write(t));
    }

    public void AppendFormatted<T>(T t, string format)
    {
        if (!colors.TryGetValue(format, out var color))
            throw new InvalidOperationException($"Color '{format}' not supported");

        actions.Add(() =>
        {
            System.Console.ForegroundColor = color;
            System.Console.Write(t);
            System.Console.ResetColor();
        });
    }

    internal void WriteLine() => Write(true);
    internal void Write() => Write(false);

    private void Write(bool newLine)
    {
        foreach (var action in actions)
            action();

        if (newLine)
            System.Console.WriteLine();
    }
}