using System.Diagnostics;
using q10;

var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
};
string filePath = files[0];
List<string> fileContent = File.ReadLines(filePath).ToList();

List<List<(Pipe Pipe, int Score, (int X, int Y)? Next)>> directions = new();
var startPosition = Question.ParsePipes(ref directions, fileContent);
var size = (W: directions.First().Count, H: directions.Count);
System.Console.WriteLine($"Start {startPosition.X}:{startPosition.Y} W:{size.W} H:{size.H}");

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

(Move move1, Move move2) startMoves = default!;
while (movesLeft-- > 0)
{
    currentPosition = moves.First().To;
    if (movesLeft % 5 == 0)
    {
        // Console.WriteLine($"MovesLeft {movesLeft} Moves gathered {moves.Count}");
    }

    // Analyze moves from current position
    var newMoves = Question.GetValidMoves(directions, currentPosition);
    if (movesLeft == 49999)
    {
        startMoves = (newMoves.First(), newMoves[1]);
    }

    // Clear current move, add new ones to list
    moves.RemoveAt(0);
    moves.AddRange(newMoves.Where(nm =>
    {
        var target = directions[nm.To.Y][nm.To.X];
        return target.Score == 0 && target.Pipe != Pipe.Start;
    }));

    // Increment scores
    Move lastmove = default;
    foreach (var m in newMoves)
    {
        lastmove = m;
        var pos = m.From;
        var toPos = m.To;
        var direction = directions[pos.Y][pos.X];
        var toDirection = directions[toPos.Y][toPos.X];
        // Console.WriteLine(
        //     $"{direction.Pipe.ToChar()}({pos.X}:{pos.Y})={direction.Score} {toDirection.Pipe.ToChar()}({toPos.X}:{toPos.Y})={toDirection.Score}");

        if (toDirection.Pipe == Pipe.Start)
        {
            // Console.WriteLine("Returned to start, not incrementing");
            // break;
        }
        else if (toDirection.Score != 0 && toDirection.Score <= direction.Score + 1)
        {
            // Console.WriteLine("ToDirection score != 0 and not smaller than proposed route");
        }
        else
        {
            // Console.WriteLine("Incrementing score");
            if (directions[pos.Y][pos.X].Next == null)
            {
                directions[pos.Y][pos.X] = (directions[pos.Y][pos.X].Pipe, directions[pos.Y][pos.X].Score, toPos);
            }

            directions[toPos.Y][toPos.X] = (directions[toPos.Y][toPos.X].Pipe, direction.Score + 1, null);
        }
    }

    // Console.WriteLine($"\nGot +{newMoves.Count} new moves from {currentPosition}, total: {moves.Count}");

    if (moves.Count == 0)
    {
        System.Console.WriteLine($"No moves left {lastmove.From} {lastmove.To}");
        break;
    }
}


// View map with ascii
// for (int i = 0; i < size.H; i++)
// {
//     for (int j = 0; j < size.W; j++)
//     {
//         var repr = directions[i][j].Score != 0 ? " " + (char)(directions[i][j].Score % (122 - 47) + 47) : " .";
//         Console.Write(repr);
//     }
//
//     Console.WriteLine();
// }

var max = directions.Max(d => d.Max(sp => sp.Score));
System.Console.WriteLine($"\nMax {max}");
// 16 is wrong
// 6831 is correct

Dictionary<int, (int X, int Y)> nodes = new();
var position = directions[startMoves.move1.From.Y][startMoves.move1.From.X];

nodes.Add(0, startPosition);

var cnt = 0;
while (true)
{
    cnt++;
    if (position.Score == 0 && cnt != 1)
    {
        throw new Exception("123");
    }

    if (position.Next != null)
    {
        nodes.Add(position.Score + 1, position.Next!.Value);
        position = directions[position.Next.Value.Y][position.Next.Value.X];
    }
    else
    {
        break;
    }
}

(Pipe Pipe, int Score, (int X, int Y)? Next) prevPosition = default;
var m2 = startMoves.move2.To;
var position2 = directions[m2.Y][m2.X];
var cnt2 = 0;
nodes.Add(max + max - 1, (m2.X, m2.Y));
while (true)
{
    cnt2++;
    if (position2.Next != null)
    {
        if (position2.Score == 0)
        {
            throw new Exception("123");
        }

        if (!nodes.TryAdd(max + (max - position2.Score - 1), position2.Next!.Value))
        {
            throw new Exception("asd");
        }

        position2 = directions[position2.Next.Value.Y][position2.Next.Value.X];
    }
    else
    {
        break;
    }
}

Debug.Assert(nodes.Count == max * 2, "TEST FAILED");
// Check for gaps
for (int i = 0; i < max * 2; i++)
{
    nodes[i] = nodes[i];
}

Debug.Assert(nodes[0] == startPosition);
Debug.Assert(Math.Abs(nodes[max * 2 - 1].X - startPosition.X) <= 1);
Debug.Assert(Math.Abs(nodes[max * 2 - 1].Y - startPosition.Y) <= 1);

var areaShoelace = 0d;
for (int i = 0; i < max * 2; i++)
{
    var n1 = nodes[i];
    var n2 = i == max * 2 - 1 ? nodes[0] : nodes[i + 1];
    areaShoelace += (1 / 2.0) * (n1.Y + n2.Y) * (n1.X - n2.X);
}

Console.Log($"cnt {cnt} cnt2 {cnt2} {cnt + cnt2}");
Console.Log($"pos {position} pos2 {position2} len {nodes.Count} shoelace A= {areaShoelace}");

var total = directions.Aggregate(0, (sum, val) => val.Count + sum);
var chars = directions.Aggregate(0, (sum, val) => val.Count(v => v.Score != 0 || v.Pipe == Pipe.Start) + sum);
var zeroes = directions.Aggregate(0, (sum, val) => val.Count(v => v.Score == 0) + sum);
var horBound = 0;
var newRows = new List<char[]>();
const char X = '!';
const char S = '.';
foreach (var row in directions)
{
    var rowStr = new String(row.Select(r => r.Score != 0 ? X : S).ToArray());
    newRows.Add(rowStr.ToArray());
    rowStr = rowStr.Trim(S);
    horBound += rowStr.Count(r => r == S);
}

for (int i = 0; i < newRows.Count; i++)
{
    for (int j = 0; j < newRows.First().Length; j++)
    {
        if (newRows[i][j] != X)
        {
            newRows[i][j] = '1';
        }
        else
        {
            break;
        }
    }

    for (int j = newRows.First().Length - 1; j >= 0; j--)
    {
        if (newRows[i][j] != X)
        {
            newRows[i][j] = '2';
        }
        else
        {
            break;
        }
    }

    for (int j = 0; j < newRows.First().Length; j++)
    {
        if (newRows[j][i] != X)
        {
            newRows[j][i] = '3';
        }
        else
        {
            break;
        }
    }

    for (int j = newRows.First().Length - 1; j >= 0; j--)
    {
        if (newRows[j][i] != X)
        {
            newRows[j][i] = '4';
        }
        else
        {
            break;
        }
    }
}

// foreach (var charArr in newRows)
// {
//     Console.WriteLine(charArr);
// }

var total2 = newRows.Aggregate(0, (sum, val) => val.Length + sum);
var chars2 = newRows.Aggregate(0, (sum, val) => val.Count(v => v == X) + sum) + 1;
var zeroes2 = newRows.Aggregate(0, (sum, val) => val.Count(v => v == S) + sum);

Console.Log($"Total {total:blue} chars {chars} zero {zeroes} horTrimmed {horBound}");
System.Console.WriteLine($"Total {total2} chars {chars2} zero {zeroes2}");

// Shoelace formula https://en.wikipedia.org/wiki/Shoelace_formula
// sum of A_i
// where A_i = 1/2 (y_i + y_i+1)(x_i + x_i+1)

// for (int y = 0; y < directions.Count; y++)
// {
//     for (int x = 0; x < directions.First().Count; x++)
//     {
//         var elem = directions[y][x];
//         if (elem.Pipe == Pipe.Start || elem.Score > 0)
//         {
//             // area += ()
//         }
//     }
// }

// Pick's theorem https://en.wikipedia.org/wiki/Pick%27s_theorem
var b = chars2;
var inside = areaShoelace - b / 2 + 1;
System.Console.WriteLine($"Inside: {inside}");