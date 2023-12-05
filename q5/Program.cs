var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
};
string filePath = files[1];
List<string> fileContent = File.ReadLines(filePath).ToList();

List<long> seeds = new();

List<string> types = new()
{
    "soil",
    "fertilizer",
    "water",
    "light",
    "temperature",
    "humidity",
};
List<Map> mappings = new();

(string Source, string Target)? processingMap = null;
foreach (var line in fileContent)
{
    if (line.Contains("seeds:"))
    {
        seeds = line.Split("seeds:")[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
    }
    else if (line.Contains(" map:"))
    {
        var toFrom = line.Replace(" map:", "").Split("-to-").ToList();
        processingMap = (toFrom.First(), toFrom.Last());
        mappings.Add(new Map()
        {
            Link = processingMap.Value,
            Mapping = new()
        });
    }
    else if (!string.IsNullOrWhiteSpace(line))
    {
        var values = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
        mappings.Last().Mapping.Add((values[1], values.First(), values.Last()));
    }
    else if (string.IsNullOrWhiteSpace(line))
    {
        if (mappings.Count > 0)
        {
            mappings.Last().Mapping.Sort((m1, m2) => m1.Source > m2.Source ? 1 : -1);
        }

        processingMap = null;
        continue;
    }
}

List<List<(long Input, long Output)>> seedIO = new();
foreach (var seed in seeds)
{
    // var lastStage = stages.LastOrDefault();

    var lastInputOutputs = new List<(long Input, long Output)> { (Input: seed, Output: 0L) };
    seedIO.Add(lastInputOutputs);

    foreach (var mapping in mappings)
    {
        var lastInputOutput = lastInputOutputs.Last();
        var lastInput = lastInputOutput.Input;
        var lessThanMapping = mapping
            .Mapping.FirstOrDefault(m =>
                // Console.WriteLine($"{m.Source < lastInput && m.Source + m.Count > lastInput}");
                m.Source < lastInput && lastInput < m.Source + m.Count
            );

        var conversion = lessThanMapping != default
            ? lessThanMapping.Target + (lastInput - lessThanMapping.Source)
            : lastInput;
        lastInputOutput!.Output = conversion;
        lastInputOutputs[^1] = lastInputOutput;
        lastInputOutputs = lastInputOutputs.Append((conversion, 0L)).ToList();
        if (mapping.Link.Target == "location")
        {
            Console.WriteLine(
                $"{mapping.Link.Source} {lastInputOutput.Input}, {mapping.Link.Target} {lastInputOutput.Output}");
        }
    }
}

public class Map
{
    public (string Source, string Target) Link { get; set; }

    public List<(long Source, long Target, long Count)> Mapping { get; set; }
}