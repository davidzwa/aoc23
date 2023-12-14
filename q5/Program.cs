var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
};
string filePath = files[0];
List<string> fileContent = File.ReadLines(filePath).ToList();

List<long> seeds = new();

List<Map> transformRows = new();

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
        transformRows.Add(new Map()
        {
            Link = processingMap.Value,
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

            // Console.WriteLine(
            //     $"{mapping.Link.Source} {lastInputOutput.Input}, {mapping.Link.Target} {lastInputOutput.Output}");
        }
    }

    return default;
}

foreach (var seed in seeds)
{
    locations.Add(ProcessLocations(seed));
}

// Console.WriteLine(locations.Min());
// Console.WriteLine();
// 51580674 correct

var seedCouples = new List<(long Start, long End)>();
for (int i = 0; i < seeds.Count; i += 2)
{
    seedCouples.Add(
        (seeds[i], seeds[i] + seeds[i + 1])
    );
}

foreach (var (start, end) in seedCouples)
{
    // List<(long Start, long Finish)> rangesToProcess = new()
    // {
    //     (start, end)
    // };
    // List<(long Start, long Finish, long LocationStart, long LocationEnd)> rangesDone = new()
    // {
    //     (start, end, 0, 0)
    // };

    Console.WriteLine($"Seed \t\t{start:n0}\tEnd {end:n0} \t\t\t\t\t\t\tDiff {end - start:n0}");

    // while (rangesToProcess.Count != 0)
    // {
    foreach (var transform in transformRows.Slice(0, 1))
    {
        var hasOverlap = false;
        var lastSourceRangeEnd = start;

        var rangeToMapLeft = end - start;

        List<(long TargetStart, long TargetEnd)> ranges = new();
        List<(long TargetStart, long TargetEnd)> referenceRanges = new();
        foreach (var range in transform.Ranges)
        {
            var startRange = range.Source;
            var endRange = range.Source + range.Count;

            var link = transform.Link;

            var startInside = start >= startRange && start <= endRange;
            var endInside = (end >= startRange && end <= endRange);
            hasOverlap |= startInside || endInside;

            var isContained = startInside && endInside;
            if (!hasOverlap)
            {
                // Console.WriteLine($"No Overlap {hasOverlap} {startOverlap} {endOverlap} {startRange:n0} to {endRange:n0} {start:n0} to {end:n0}");
                continue;
            }

            // The next assumes void needs to be mapped as well, Im skipping that for now
            // If we have a hit, we should add the range on the left of the hit as direct mapping range
            // if (ranges.Count == 0 && start > 0)
            // {
            //     ranges.Add((0, start - 1));
            // }

            var prefix =
                $"{link.Source}-{link.Target} Start {startRange:n0}\tEnd {endRange:n0}";
            if (isContained)
            {
                var rangeCount = end - start;
                var targetStart = range.Target + (start - startRange);
                var previousRange = (TargetStart: start, TargetEnd: end);
                (long TargetStart, long TargetEnd) addedRange = (targetStart, targetStart + rangeCount);
                rangeToMapLeft -= rangeCount;
                ranges.Add(addedRange);
                referenceRanges.Add(previousRange);
                Console.WriteLine($"{prefix} Contained {previousRange} {addedRange}");

                // We've found the end of the range
                break;
            }

            // ... [1 ... 2] ... [ 3 ... 4] ...
            // I2  I1  LO  LO2  RO2   RO  O1    O2
            // I2-I1, LO2-RO2, O1-O2 should be added as direct range as well
            // LO2-RO2 can be: null (contained), partial (right or left overlap) or overfull (multi-range)

            // With a previous Right-Overlap we could be skipping untransformed gaps per accident
            // Ranges 5-8 11-14 seed 6-12 could result in (6,8,?,,?) (11,12,?,,?) meaning 9-10 is left unmapped 
            if (startInside)
            {
                var rangeCount = endRange - start;
                var targetStart = range.Target + (start - startRange);
                var endMapped = targetStart + rangeCount;
                var previousRange = (TargetStart: start, TargetEnd: start + rangeCount);
                var addedRange = (TargetStart: targetStart, TargetEnd: endMapped);
                rangeToMapLeft -= rangeCount;
                ranges.Add(addedRange);
                referenceRanges.Add(previousRange);

                Console.WriteLine(
                    $"{prefix} Left-Overlap  {previousRange} {addedRange} \tDiff {addedRange.TargetEnd - addedRange.TargetStart:n0}");
                lastSourceRangeEnd = endRange;
                // Console.WriteLine($"Set lastSourceRangeEnd to {lastSourceRangeEnd}");
                continue;
            }

            if (endInside)
            {
                // TODO add range for LO2-RO2 on the left side
                var newStart = lastSourceRangeEnd + 1;
                var newEnd = startRange - 1;
                var rangeCountLo2 = newEnd - newStart;
                var directMapTargetStart = newStart;
                var endDirect = directMapTargetStart + rangeCountLo2;
                // Console.WriteLine($"Got lastSourceRangeEnd to {newStart}");
                var addedRangeDirectMap = (TargetStart: directMapTargetStart, TargetEnd: endDirect);
                rangeToMapLeft -= rangeCountLo2;
                ranges.Add(addedRangeDirectMap);
                referenceRanges.Add(addedRangeDirectMap);
                var prefixSpecial =
                    $"{link.Source}-{link.Target} Start {directMapTargetStart:n0}\tEnd {endDirect:n0}";
                Console.WriteLine(
                    $"{prefixSpecial} Direct-Map    {addedRangeDirectMap} {addedRangeDirectMap} \tDiff {addedRangeDirectMap.TargetEnd - addedRangeDirectMap.TargetStart:n0}");

                if (endDirect != newEnd)
                {
                    throw new Exception($"Wrong ending direct range {endDirect:n0} {newEnd:n0}");
                }

                var rangeCount = end - startRange;
                var targetStart = range.Target + (start - startRange);
                var targetEnd = targetStart + rangeCount;
                var addedRange = (TargetStart: targetStart, TargetEnd: targetEnd);
                rangeToMapLeft -= rangeCount;
                var previousRange = (TargetStart: startRange, TargetEnd: startRange + rangeCount);
                ranges.Add(addedRange);
                referenceRanges.Add(previousRange);
                if (addedRange.TargetEnd - addedRange.TargetStart <= 0)
                {
                    throw new Exception(
                        $"Wrong ending direct range {addedRange.TargetEnd - addedRange.TargetStart:n0} <=0");
                }

                Console.WriteLine(
                    $"{prefix} Right-Overlap {previousRange} {addedRange} \tDiff {addedRange.TargetEnd - addedRange.TargetStart:n0}");

                // We've found the end of the splitrange
                break;
            }
            else
            {
                throw new Exception("Unfinished FULL OVERLAP");
                // var rangeCount = endRange - startRange;
                // var targetStart = range.Target;
                // // TODO add range for LO2-RO2 on the left side, let right side be fixed by post-loop or right-side overlap 
                // (long TargetStart, long TargetEnd) addedRange = (targetStart, targetStart + rangeCount);
                // rangeToMapLeft -= rangeCount;
                // ranges.Add(addedRange);
                // referenceRanges.Add(previousRange);
                // hasOverlap = true;
                // Console.WriteLine(
                //     $"FULL OVERLAP {addedRange} \tDiff {addedRange.TargetEnd - addedRange.TargetStart:n0}");
            }
        }

        // TODO check if not direct mapping left (previous left-overlap)
        var checkDiff = referenceRanges.Last().TargetEnd - referenceRanges.First().TargetStart;
        var seedDiff = end - start;
        if (referenceRanges.Count >= 1 && checkDiff != seedDiff)
        {
            throw new Exception($"Mapped range end-start not equal to original end-start {checkDiff} {seedDiff}");
        }
        if (referenceRanges.Count == 0)
        {
            throw new Exception("No range mapped");
        }
        
        if (rangeToMapLeft != ranges.Count - 1)
        {
            throw new Exception($"Range to map is not empty {rangeToMapLeft}");
        }



        if (!hasOverlap)
        {
            throw new Exception("No overlap, map directly one-to-one, unfinished");
        }

        foreach (var range in ranges)
        {
            Console.WriteLine($"Range {range}");
        }
    }
    
    Console.WriteLine("-- Done with seed. Commit and continue to next seed\n");
}

return;
// List<long> locations2 = new();
// This is very slow and also very stupid
// 3 scenario's
// 1 - range is completely outside seed-ranges => keep range as is (lucky)
// 2 - range is completely in seed-range => map range to new range (lucky)
// 3 - range is partially in seed-ranges => split range into sub-range
// foreach (var seed in seedCouples)
// {
//     Console.WriteLine($"Range {seed}");
//     
//     
//     for (long i = seed.Start; i < seed.Start + seed.Count; i++)
//     {
//         // var lastStage = stages.LastOrDefault();
//         var lastInputOutputs = new List<(long Input, long Output)> { (Input: i, Output: 0L) };
//         seedIO.Add(lastInputOutputs);
//
//         foreach (var mapping in mappings)
//         {
//             var lastInputOutput = lastInputOutputs.Last();
//             var lastInput = lastInputOutput.Input;
//             var lessThanMapping = mapping
//                 .Mapping.FirstOrDefault(m =>
//                     m.Source <= lastInput && lastInput < m.Source + m.Count
//                 );
//
//             var conversion = lessThanMapping != default
//                 ? lessThanMapping.Target + (lastInput - lessThanMapping.Source)
//                 : lastInput;
//             lastInputOutput!.Output = conversion;
//             lastInputOutputs[^1] = lastInputOutput;
//             lastInputOutputs = lastInputOutputs.Append((conversion, 0L)).ToList();
//             if (mapping.Link.Target == "location" || mapping.Link.Target == "soil")
//             {
//                 if (mapping.Link.Target == "location")
//                 {
//                     locations2.Add(lastInputOutput.Output);
//                     break;
//                 }
//
//                 // Console.WriteLine(
//                 //     $"{mapping.Link.Source} {lastInputOutput.Input}, {mapping.Link.Target} {lastInputOutput.Output}");
//             }
//         }
//     }
// }
// Console.WriteLine(locations2.Min());

public class Map
{
    public (string Source, string Target) Link { get; set; }

    public List<(long Source, long Target, long Count)> Ranges { get; set; }
}