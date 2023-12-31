namespace q10;

public class Question
{
    public static (int X, int Y) ParsePipes(ref List<List<(Pipe Direction, int Score)>> directions, List<string> lines)
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

    public static List<Move> GetValidMoves(ref List<List<(Pipe Direction, int Score)>> directions,
        (int X, int Y) pos)
    {
        var size = (W: directions.First().Count, H: directions.Count);
        var newMoves = new List<Move>();
        
        // TODO check top(if pos.y > 0), left(if pos is >0),right(if pos is <width-1), bottom (if pos.y < h-1)  
        pos.X
        
        return newMoves;
    }

    static bool IsValidMove(Pipe from, Pipe to, Direction direction)
    {
        return true;
    } 
    
    static Pipe ToPipe(char pipe) => pipe switch
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

    public enum Direction
    {
        Top,
        Bottom,Left,Right
    }
}