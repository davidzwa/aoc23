var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
};
string filePath = files[0];
List<string> fileContent = File.ReadLines(filePath).ToList();

var sumTotalCards = 0;
List<int> cardWinningNumberCount = new();
foreach (var cards in fileContent)
{
    var indexGame = cards.Split(": ").ToList();
    var gameIndex = int.Parse(indexGame[0].Replace("Card ", ""));
    var winAndContents = indexGame[1].Split(" | ").ToList();
    if (!winAndContents.Count.Equals(2))
    {
        throw new Exception("invalid parse output");
    }

    var winners = winAndContents[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
    var numbers = winAndContents[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
    var cardWins = winners.Sum(w => numbers.Contains(w) ? 1 : 0);
    var cardScore = (int)Math.Pow(2, cardWins - 1);

    sumTotalCards += cardScore;
    cardWinningNumberCount.Add(cardWins);
    Console.WriteLine($"gameIndex {gameIndex} cardWins: {cardWins} (score: {cardScore})");
}

Console.WriteLine(sumTotalCards);
// 18619 correct

List<int> cardCounts = Enumerable.Repeat(1, cardWinningNumberCount.Count).ToList();
for (var i = 0; i < cardCounts.Count; i++)
{
    var reps = cardWinningNumberCount[i];
    if (i == cardCounts.Count - 1)
        continue;

    // 0-based, inclusive max
    var endIndex = Math.Min(i + reps, cardCounts.Count - 1);
    Console.WriteLine($"i: {i} reps: {reps} endIndex: {endIndex}");
    for (var j = i + 1; j <= endIndex; j++)
    {
        cardCounts[j] += cardCounts[i];
        Console.WriteLine($"j {j} reps {reps}");
    }
}

// 1, 2, 4, 8, 14, 1
Console.WriteLine(cardCounts.Sum());