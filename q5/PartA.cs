namespace q5;

public static class PartA
{
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