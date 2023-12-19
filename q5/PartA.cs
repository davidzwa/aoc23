namespace q5;

public static class PartA
{
    public static (List<long> Seeds, List<Map> Transformers) ParseFile(List<string> fileContent)
    {
        List<Map> transformRows = new();
        List<long> seeds = new();

        foreach (var line in fileContent)
        {
            if (line.Contains("seeds:"))
            {
                seeds = line.Split("seeds:")[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
            }
            else if (line.Contains(" map:"))
            {
                var toFrom = line.Replace(" map:", "").Split("-to-").ToList();
                transformRows.Add(new Map()
                {
                    Link = (toFrom.First(), toFrom.Last()),
                    Ranges = new()
                });
            }
            else if (!string.IsNullOrWhiteSpace(line))
            {
                var values = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
                transformRows.Last().Ranges.Add((values[1], values.First(), values.Last()));
            }
            else if (string.IsNullOrWhiteSpace(line))
            {
                if (transformRows.Count > 0)
                {
                    transformRows.Last().Ranges.Sort((m1, m2) => m1.Source > m2.Source ? 1 : -1);
                }
            }
        }

        return (seeds, transformRows);
    }
    
    public static void Process(List<Map> transformRows)
    {
        List<List<(long Input, long Output)>> seedIO = new();
        List<long> locations = new();

        long ProcessLocations(long seed)
        {
            var lastInputOutputs = new List<(long Input, long Output)> { (Input: seed, Output: 0L) };
            seedIO.Add(lastInputOutputs);

            foreach (var mapping in transformRows)
            {
                var lastInputOutput = lastInputOutputs.Last();
                var lastInput = lastInputOutput.Input;
                var lessThanMapping = mapping
                    .Ranges.FirstOrDefault(m =>
                        m.Source <= lastInput && lastInput < m.Source + m.Count
                    );

                var conversion = lessThanMapping != default
                    ? lessThanMapping.Target + (lastInput - lessThanMapping.Source)
                    : lastInput;
                lastInputOutput!.Output = conversion;
                lastInputOutputs[^1] = lastInputOutput;
                lastInputOutputs = lastInputOutputs.Append((conversion, 0L)).ToList();
                if (mapping.Link.Target == "location" || mapping.Link.Target == "soil")
                {
                    if (mapping.Link.Target == "location")
                    {
                        return lastInputOutput.Output;
                    }
                }
            }

            return default;
        }

        // foreach (var seed in seeds)
        // {
        //     locations.Add(ProcessLocations(seed));
        // }

        // Console.WriteLine(locations.Min());
        // Console.WriteLine();
        // 51580674 correct
    }
}