var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
};
string filePath = files[1];
List<string> fileContent = File.ReadLines(filePath).ToList();

var sumTotalCards = 0;
List<int> cardScores = new();
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
    cardScores.Add(cardWins);
    Console.WriteLine($"gameIndex {gameIndex} cardWins: {cardWins} (score: {cardScore})");
}

List<int> cardCounts = 1..4;

Console.WriteLine(sumTotalCards);
// 18619 correct

