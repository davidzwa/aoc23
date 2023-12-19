using q5;

var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
    "input2.txt",
};
string filePath = files[3];
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

// PartA.Process(transformRows);


var seedCouples = new List<(long Start, long End)>();
for (int i = 0; i < seeds.Count; i += 2)
{
    seedCouples.Add(
        (seeds[i], seeds[i] + seeds[i + 1])
    );
}

(bool Added, long Diff) AddToRanges(
    ref List<(long TargetStart, long TargetEnd)> ranges,
    ref List<(long Start, long End)> refRanges,
    long start,
    long targetStart,
    long rangeCount)
{
    var targetEnd = targetStart + rangeCount;
    var rangeLength = targetEnd - targetStart;
    if (rangeLength == -2)
    {
        return (false, 0);
    }

    if (rangeLength <= 0)
    {
        throw new Exception($"Illegal rangeLength <=0 {rangeLength}");
    }

    if (rangeLength == 1)
    {
        throw new Exception("Edge case rangeLength == 1");
    }

    (long TargetStart, long TargetEnd) addedRange = (targetStart, targetEnd);
    ranges.Add(addedRange);

    var refEnd = start + rangeCount;
    (long Start, long End) addedRefRange = (start, refEnd);
    refRanges.Add(addedRefRange);

    var refRangeLength = refEnd - start + 1;
    if (refRangeLength <= 0)
    {
        throw new Exception("Illegal refRangeLength <=0");
    }

    if (refRangeLength == 1)
    {
        throw new Exception("Edge case refRangeLength == 1");
    }

    return (true, rangeLength);
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
            var prefix =
                $"{link.Source}-{link.Target} Start {startRange:n0}\tEnd {endRange:n0}";
            
            var startInside = start >= startRange && start <= endRange;
            var endInside = (end >= startRange && end <= endRange);
            hasOverlap |= startInside || endInside;

            var isContained = startInside && endInside;
            if (!hasOverlap)
            {
                // Console.WriteLine($"No Overlap {startRange:n0} to {endRange:n0} {start:n0} to {end:n0}");
                continue;
            }

            if (isContained)
            {
                var rangeCount = end - start;
                var targetStart = range.Target + (start - startRange);
                (bool added, long diff) = AddToRanges(ref ranges, ref referenceRanges, start, targetStart, rangeCount);
                if (added)
                {
                    rangeToMapLeft -= diff;
                    Console.WriteLine($"{prefix} Contained {referenceRanges.Last()} {ranges.Last()}");
                    lastSourceRangeEnd = endRange;
                }
                else
                {
                    throw new Exception("Contained should always add");
                }

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

                (bool added, long diff) = AddToRanges(ref ranges, ref referenceRanges, start, targetStart, rangeCount);
                if (added)
                {
                    rangeToMapLeft -= diff;
                    Console.WriteLine(
                        $"{prefix} Left-Overlap  {referenceRanges.Last()} {ranges.Last()} \tDiff {ranges.Last().TargetEnd - ranges.Last().TargetStart:n0}");
                    lastSourceRangeEnd = endRange;
                }

                continue;
            }

            if (endInside)
            {
                {
                    var newStart = lastSourceRangeEnd + 1;
                    var newEnd = startRange - 1;
                    var rangeCountLo2 = newEnd - newStart;
                    (bool added, long diff) =
                        AddToRanges(ref ranges, ref referenceRanges, newStart, newStart, rangeCountLo2);
                    rangeToMapLeft -= diff;
                    if (added)
                    {
                        var prefixSpecial =
                            $"{link.Source}-{link.Target} Start {newStart:n0}\tEnd {newEnd:n0}";
                        Console.WriteLine(
                            $"{prefixSpecial} Direct-Map    {ranges.Last()} {ranges.Last()} \tDiff {ranges.Last().TargetEnd - ranges.Last().TargetStart:n0}");
                        if (ranges.Last().TargetEnd != newEnd)
                        {
                            throw new Exception($"Wrong ending direct range {ranges.Last().TargetEnd:n0} {newEnd:n0}");
                        }

                        lastSourceRangeEnd = newEnd;
                    }
                }

                {
                    var rangeCount = end - startRange;
                    var targetStart = range.Target + (start - startRange);
                    (bool added2, long diff2) =
                        AddToRanges(ref ranges, ref referenceRanges, startRange, targetStart, rangeCount);
                    if (added2)
                    {
                        rangeToMapLeft -= diff2;

                        if (ranges.Last().TargetEnd - ranges.Last().TargetStart <= 0)
                        {
                            throw new Exception(
                                $"Wrong ending direct range {ranges.Last().TargetEnd - ranges.Last().TargetStart:n0} <=0");
                        }

                        Console.WriteLine(
                            $"{prefix} Right-Overlap {referenceRanges.Last()} {ranges.Last()} \tDiff {ranges.Last().TargetEnd - ranges.Last().TargetStart:n0}");
                        lastSourceRangeEnd = endRange;
                    }
                }

                // We've found the end of the splitrange
                break;
            }
            else
            {
                {
                    // FULL OVERLAP with direct-left, full range and the right part is either next iteration or post-loop administration
                    var newStart = lastSourceRangeEnd + 1;
                    var newEnd = startRange - 1;
                    var rangeCountLo2 = newEnd - newStart;
                    (bool added, long diff) =
                        AddToRanges(ref ranges, ref referenceRanges, newStart, newStart, rangeCountLo2);
                    if (added)
                    {
                        rangeToMapLeft -= diff;
                        var prefixSpecial =
                            $"{link.Source}-{link.Target} Start {newStart:n0}\tEnd {newEnd:n0}";
                        Console.WriteLine(
                            $"{prefixSpecial} Direct-Map    {ranges.Last()} {ranges.Last()} \tDiff {ranges.Last().TargetEnd - ranges.Last().TargetStart:n0}");

                        if (ranges.Last().TargetEnd != newEnd)
                        {
                            throw new Exception($"Wrong ending direct range {ranges.Last().TargetEnd:n0} {newEnd:n0}");
                        }

                        lastSourceRangeEnd = endRange;
                    }
                    else
                    {
                        Console.WriteLine($"No direct mapping. Last end {lastSourceRangeEnd:n0}");
                    }
                }

                {
                    var rangeCount = end - start;
                    var targetStart = range.Target + (start - startRange);
                    (bool added, long diff) =
                        AddToRanges(ref ranges, ref referenceRanges, start, targetStart, rangeCount);
                    if (added)
                    {
                        rangeToMapLeft -= diff;
                        Console.WriteLine($"{prefix} Full Overlap {referenceRanges.Last()} {ranges.Last()}");
                        lastSourceRangeEnd = endRange;
                    }
                    else
                    {
                        throw new Exception("Unfinished FULL OVERLAP");
                    }
                }
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
            throw new Exception($"Range to map is not empty {rangeToMapLeft} != -1");
        }

        if (!hasOverlap)
        {
            throw new Exception("No overlap, map directly one-to-one, unfinished");
        }

    }

    Console.WriteLine("--\n");
}

// Console.WriteLine(locations2.Min());