using q10;

var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
};
string filePath = files[1];
List<string> fileContent = File.ReadLines(filePath).ToList();

List<List<(Pipe Direction, int Score)>> directions = new();
var startPosition = Question.ParsePipes(ref directions, fileContent);

Console.WriteLine(startPosition.X + " " + startPosition.Y);

// Breadth-first, at start create list of current moves
// list valid options, save incremented score to all if existing score is 0 or if non-zero + smaller, add to moves-list if score is zero
// remove current move
// iterate moves list until no moves

int movesLeft = 10;
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
        Console.WriteLine($"MovesLeft {movesLeft}");
    }

    // Analyze moves from current position
    var newMoves = Question.GetValidMoves(ref directions, currentPosition);

    if (moves.Count == 0)
    {
        Console.WriteLine("No moves left");
        break;
    }
}

Console.WriteLine($"\nMax {directions.Max(d => d.Max())}");

public class Move
{
    public (int x, int y) From { get; set; }
    public (int x, int y) To { get; set; }
}