// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;

// Read a file
var files = new[]
{
    "input.txt",
    "example2.txt",
    "test1.txt",
};
string filePath = files[0];
List<string> fileContent = File.ReadLines(filePath).ToList();

var sum = 0;
var partOne = false;
var partTwo = true;
var digits = new List<(string name, int value)>
{
    ("one", 1),
    ("two", 2),
    ("three", 3),
    ("four", 4),
    ("five", 5),
    ("six", 6),
    ("seven", 7),
    ("eight", 8),
    ("nine", 9),
    ("1", 1),
    ("2", 2),
    ("3", 3),
    ("4", 4),
    ("5", 5),
    ("6", 6),
    ("7", 7),
    ("8", 8),
    ("9", 9),
};

if (partOne)
{
    foreach (var line in fileContent)
    {
        // Tactic 1 - convert string to int with regex whilst filtering alphabetical
        var number = Regex.Replace(line, "[^0-9]", "");
        // Tactic 2 - convert string to int with LINQ (seems expensive)
        var number2 = string.Join("", line.Where(c => int.TryParse(c.ToString(), out _)));
        // Tactic 3 - convert string by subtracting 48 and checking if it is between 0 and 9
        var number3 = string.Join("", line.Where(c => c - 48 >= 0 && c - 48 <= 9));

        if (number != number2 || number != number3)
        {
            throw new Exception("Conversion failed! " + line);
        }

        var numberUsed = number3.First().ToString() + number3.Last();

        if (string.IsNullOrWhiteSpace(numberUsed))
        {
            Console.WriteLine("Number is not right: " + numberUsed);
            continue;
        }

        var i = int.Parse(numberUsed);
        sum += i;
    }

    // 23966707 too high
    // correct 54667
    Console.WriteLine("Sum: " + sum);
    Console.WriteLine("Linecount: " + fileContent.Count);
}

// var lastIndex1 = "nine6five181".LastIndexOf("1", StringComparison.InvariantCulture);
// Console.WriteLine(lastIndex1);
// return 1;

// Part 2
if (partTwo)
{
    var sumPartTwo = 0;
    foreach (var line in fileContent)
    {
        var firstIndex = line.Length + 1;
        int? firstDigit = null;
        var lastIndex = 0;
        int? lastDigit = null;
        foreach (var (entry, value) in digits)
        {
            var foundIndex = line.IndexOf(entry, StringComparison.InvariantCulture);
            if (foundIndex != -1 && foundIndex < firstIndex)
            {
                firstIndex = foundIndex;
                firstDigit = value;
            }

            var lastFoundIndex = line.LastIndexOf(entry, StringComparison.InvariantCulture);
            if (lastFoundIndex != -1 && lastFoundIndex > lastIndex)
            {
                lastIndex = lastFoundIndex;
                lastDigit = value;
                // Console.WriteLine("Last index: " + lastIndex + " " + entry + " " + value + " " + line);
            }
        }

        var onlyOne = firstIndex == lastIndex;
        if (firstIndex == line.Length || lastIndex == line.Length + 1 || firstIndex > lastIndex)
        {
            throw new Exception("Conversion failed! " + line);
        }
        
        var numberUsed = onlyOne ? firstDigit.ToString() + firstDigit : firstDigit.ToString() + lastDigit;
        Console.WriteLine("Number used " + numberUsed + " " + line);

        if (string.IsNullOrWhiteSpace(numberUsed))
        {
            Console.WriteLine("Number is not right: " + numberUsed);
            continue;
        }

        if (line.Equals("vggvnhqkjseventwo4onetwonftrnd"))
        {
            Console.WriteLine("Test case failed" + numberUsed);
        }

        var i = int.Parse(numberUsed);
        sumPartTwo += i;
    }
    
    // 66687 too high - I overwrote lastIndex and firstIndex with Min (fine), but also set the lastDigit
    // 50033 too low - 'nine6five181 becomes 98 - set lastIndex to foundIndex instead of lastFoundIndex
    // 51513 too low - I didnt know about duplication of first digit
    // 54203 correct
    
    Console.WriteLine("Sum (part two): " + sumPartTwo);
    Console.WriteLine("Linecount: " + fileContent.Count);
}