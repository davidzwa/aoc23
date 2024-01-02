namespace q10;

public static class Question
{
    public static (int X, int Y) ParsePipes(ref List<List<(Pipe Pipe, int Score)>> directions, List<string> lines)
    {
        directions.Clear();

        (int X, int Y) startPosition = (-1, -1);
        foreach (var (y, line) in lines.Select((value, index) => (index, value)))
        {
            directions.Add(new());

            var row = directions.Last();
            foreach (var (x, charPipe) in line.Select((value, index) => (index, value)))
            {
                var pipe = ToPipe(charPipe);
                if (pipe == Pipe.Start)
                {
                    startPosition = (x, y);
                }

                row.Add(new(pipe, 0));
            }
        }

        if (startPosition.X == -1 && startPosition.Y == -1)
        {
            throw new Exception("Start position not found");
        }

        return startPosition;
    }

    public static List<Move> GetValidMoves(List<List<(Pipe Pipe, int Score)>> directions,
        (int X, int Y) pos)
    {
        var size = (W: directions.First().Count, H: directions.Count);

        var direction = directions[pos.Y][pos.X];
        var absOptions = ToPipeOptions(direction.Pipe).Select(o => (X: pos.X + o.X, Y: pos.Y + o.Y)).ToList();
        var relevantOptions = absOptions.Where(c =>
        {
            var wMin = c.X >= 0;
            var wMax = c.X < size.W;
            var hMin = c.Y >= 0;
            var hMax = c.Y < size.H;
            var isInRect = wMin && wMax && hMin && hMax;
            if (!wMax || !hMax)
            {
                Console.WriteLine(
                    $"W:{size.W} H:{size.H} {ToChar(direction.Pipe)}({pos.X}:{pos.Y}) ({c.X}:{c.Y}) Invalid");
            }

            return isInRect;
        }).ToList();

        return relevantOptions
            .Where(roPos =>
            {
                var toDirectionOption = directions[roPos.Y][roPos.X];
                var targets = ToPipeOptions(toDirectionOption.Pipe).Select(o => (X: roPos.X + o.X, Y: roPos.Y + o.Y))
                    .ToList();
                // The connecting pipe must have a target that is equal to the current position we're trying to move from
                var overlaps = targets.Any(t => t == pos);
                return overlaps;
            })
            .Select(ro => new Move()
            {
                From = pos,
                To = ro,
            }).ToList();
    }

    public static List<(int X, int Y)> ToPipeOptions(Pipe pipe) => pipe switch
    {
        Pipe.Vert => new()
        {
            (0, -1),
            (0, 1)
        },
        Pipe.Hor => new()
        {
            (-1, 0),
            (1, 0)
        },
        Pipe.NorthEastL => new()
        {
            (0, -1),
            (1, 0)
        },
        Pipe.NorthWestJ => new()
        {
            (0, -1),
            (-1, 0)
        },
        Pipe.SouthWest7 => new()
        {
            (0, 1),
            (-1, 0)
        },
        Pipe.SouthEastF => new()
        {
            (0, 1),
            (1, 0)
        },
        Pipe.Ground => new(),
        Pipe.Start => new List<Pipe>() { Pipe.Hor, Pipe.Vert }.SelectMany(ToPipeOptions).ToList(),
        _ => throw new Exception("Pipe not recognized"),
    };

    public static bool IsValidMove(Pipe from, Pipe to, Direction direction)
    {
        return true;
    }

    public static Pipe ToPipe(this char pipe) => pipe switch
    {
        '|' => Pipe.Vert,
        '-' => Pipe.Hor,
        'L' => Pipe.NorthEastL,
        'J' => Pipe.NorthWestJ,
        '7' => Pipe.SouthWest7,
        'F' => Pipe.SouthEastF,
        '.' => Pipe.Ground,
        'S' => Pipe.Start,
        _ => throw new Exception("Pipe not recognized"),
    };

    public static char ToChar(this Pipe pipe) => pipe switch
    {
        Pipe.Vert => '|',
        Pipe.Hor => '-',
        Pipe.NorthEastL => 'L',
        Pipe.NorthWestJ => 'J',
        Pipe.SouthWest7 => '7',
        Pipe.SouthEastF => 'F',
        Pipe.Ground => '.',
        Pipe.Start => 'S',
        _ => throw new Exception("Pipe not recognized"),
    };

    public enum Direction
    {
        Top,
        Bottom,
        Left,
        Right
    }
}