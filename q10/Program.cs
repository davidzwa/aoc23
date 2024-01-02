using q10;

var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
};
string filePath = files[0];
List<string> fileContent = File.ReadLines(filePath).ToList();

List<List<(Pipe Pipe, int Score)>> directions = new();
var startPosition = Question.ParsePipes(ref directions, fileContent);
var size = (W: directions.First().Count, H: directions.Count);
Console.WriteLine($"Start {startPosition.X}:{startPosition.Y} W:{size.W} H:{size.H}");

// Breadth-first, at start create list of current moves
// list valid options, save incremented score to all if existing score is 0 or if non-zero + smaller, add to moves-list if score is zero
// remove current move
// iterate moves list until no moves

int movesLeft = 50000;
var currentPosition = startPosition;
List<Move> moves = new()
{
    new()

    {
        From = startPosition,
        To = startPosition
    }
};

while (movesLeft-- > 0)
{
    currentPosition = moves.First().To;
    if (movesLeft % 5 == 0)
    {
        Console.WriteLine($"MovesLeft {movesLeft} Moves gathered {moves.Count}");
    }

    // Analyze moves from current position
    var newMoves = Question.GetValidMoves(directions, currentPosition);
    // Clear current move, add new ones to list
    moves.RemoveAt(0);
    moves.AddRange(newMoves.Where(nm =>
    {
        var target = directions[nm.To.Y][nm.To.X];
        return target.Score == 0 && target.Pipe != Pipe.Start;
    }));
    
    // Increment scores
    foreach (var m in newMoves)
    {
        var pos = m.From;
        var toPos = m.To;
        var direction = directions[pos.Y][pos.X];
        var toDirection = directions[toPos.Y][toPos.X];
        Console.WriteLine(
            $"{direction.Pipe.ToChar()}({pos.X}:{pos.Y})={direction.Score} {toDirection.Pipe.ToChar()}({toPos.X}:{toPos.Y})={toDirection.Score}");

        if (toDirection.Pipe == Pipe.Start)
        {
            // Console.WriteLine("Returned to start, not incrementing");
        }
        else if (toDirection.Score != 0 && toDirection.Score <= direction.Score + 1)
        {
            // Console.WriteLine("ToDirection score != 0 and not smaller than proposed route");
        }
        else
        {
            // Console.WriteLine("Incrementing score");
            directions[toPos.Y][toPos.X] = (directions[toPos.Y][toPos.X].Pipe, direction.Score + 1);
        }
    }    

    
    Console.WriteLine($"\nGot +{newMoves.Count} new moves from {currentPosition}, total: {moves.Count}");

    if (moves.Count == 0)
    {
        Console.WriteLine("No moves left");
        break;
    }
}

for (int i = 0; i < size.H; i++)
{
    for (int j = 0; j < size.W; j++)
    {
        var repr = directions[i][j].Score != 0 ? " " + directions[i][j].Score: " .";
        Console.Write(repr);
    }
    Console.WriteLine();
}

Console.WriteLine($"\nMax {directions.Max(d => d.Max(sp => sp.Score))}");
// 16 is wrong
// 6831 is correct
public class Move
{
    public (int X, int Y) From { get; set; }
    public (int X, int Y) To { get; set; }
}