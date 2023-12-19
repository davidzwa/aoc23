using q5;

var files = new[]
{
    "input.txt",
    "example.txt",
    "test1.txt",
    "input2.txt",
};
string filePath = files[0];
List<string> fileContent = File.ReadLines(filePath).ToList();

(List<long> seeds, List<Map> transformers) = PartA.ParseFile(fileContent);
// PartA.Process(transformRows);

var seedCouples = new List<(long Start, long End)>();
for (int i = 0; i < seeds.Count; i += 2)
{
    seedCouples.Add(
        (seeds[i], seeds[i] + seeds[i + 1] - 1)
    );
}

(bool Added, long Diff) AddToRanges(
    ref List<(long TargetStart, long TargetEnd)> ranges,
    ref List<(long Start, long End)> refRanges,
    long refStart,
    long targetStart,
    long rangeCount)
{
    if (targetStart == refStart)
    {
        // Console.WriteLine("Target == Start. Ensure direct mapping or gapless");
    }

    var targetEnd = targetStart + rangeCount - 1;
    var rangeLength = targetEnd - targetStart + 1;
    if (rangeLength == 0)
    {
        return (false, 0);
    }

    if (rangeLength <= 0)
    {
        throw new Exception($"Illegal rangeLength <=0 {rangeLength}");
    }

    // if (rangeLength == 1)
    // {
    //     throw new Exception("Edge case rangeLength == 1");
    // }

    (long TargetStart, long TargetEnd) addedRange = (targetStart, targetEnd);
    ranges.Add(addedRange);

    var refEnd = refStart + rangeCount - 1;
    var refRangeLength = refEnd - refStart + 1;
    if (refRangeLength <= 0)
    {
        throw new Exception("Illegal refRangeLength <=0");
    }

    // if (refRangeLength == 1)
    // {
    //     throw new Exception("Edge case refRangeLength == 1");
    // }

    (long Start, long End) addedRefRange = (refStart, refEnd);
    refRanges.Add(addedRefRange);

    return (true, rangeLength);
}

(List<(long TargetStart, long TargetEnd)> Ranges, List<(long TargetStart, long TargetEnd)> ReferenceRanges)
    ProcessRange(Map transform, long start, long end)
{
    List<(long TargetStart, long TargetEnd)> ranges = new();
    List<(long TargetStart, long TargetEnd)> referenceRanges = new();

    var hasOverlap = false;
    var lastSourceRangeEnd = start - 1;
    var rangeToMapLeft = end - start + 1;
    var link = transform.Link;
    foreach (var range in transform.Ranges)
    {
        var startRange = range.Source;
        var endRange = range.Source + range.Count - 1;

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
            Console.WriteLine("Contained");
            
            var rangeCount = end - start + 1;
            var targetStart = range.Target + (start - startRange);
            (bool added, long diff) = AddToRanges(ref ranges, ref referenceRanges, start, targetStart, rangeCount);
            if (added)
            {
                rangeToMapLeft -= diff;
                if (rangeToMapLeft < 0)
                {
                    throw new Exception("RangeToMap has become negative");
                }

                Console.WriteLine($"{prefix} Contained {referenceRanges.Last()} {ranges.Last()}");
                // lastSourceRangeEnd = endRange;
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
            Console.WriteLine("Left-Overlap");
            
            var rangeCount = endRange - start + 1;
            var targetStart = range.Target + (start - startRange);

            (bool added, long diff) = AddToRanges(ref ranges, ref referenceRanges, start, targetStart, rangeCount);
            if (added)
            {
                rangeToMapLeft -= diff;
                if (rangeToMapLeft < 0)
                {
                    throw new Exception("RangeToMap has become negative");
                }

                Console.WriteLine(
                    $"{prefix} Left-Overlap  {referenceRanges.Last()} {ranges.Last()} \tDiff {ranges.Last().TargetEnd - ranges.Last().TargetStart:n0}");
                lastSourceRangeEnd = start + rangeCount - 1;
                Console.WriteLine($"Set lastSourceRangeEnd {lastSourceRangeEnd} ");
            }
            else
            {
                Console.WriteLine("Range empty. Not added");
            }

            continue;
        }

        if (endInside)
        {
            Console.WriteLine("Right-Overlap");
            {
                var newStart = lastSourceRangeEnd + 1;
                var newEnd = startRange - 1;
                var rangeCountLo2 = newEnd - newStart + 1;
                (bool added, long diff) =
                    AddToRanges(ref ranges, ref referenceRanges, newStart, newStart, rangeCountLo2);
                rangeToMapLeft -= diff;
                if (rangeToMapLeft < 0)
                {
                    throw new Exception("RangeToMap has become negative");
                }

                if (added)
                {
                    var prefixSpecial =
                        $"{link.Source}-{link.Target} Start {newStart:n0}\tEnd {newEnd:n0}";
                    Console.WriteLine(
                        $"{prefixSpecial} Direct-Map    {ranges.Last()} {ranges.Last()} \tDiff {ranges.Last().TargetEnd - ranges.Last().TargetStart:n0}");
                    if ((ranges.Last().TargetEnd - ranges.Last().TargetStart > 0) && ranges.Last().TargetEnd != newEnd)
                    {
                        throw new Exception($"Wrong ending direct range {ranges.Last().TargetEnd:n0} {newEnd:n0}");
                    }
                }
            }

            {
                var rangeCount = end - startRange + 1;
                var targetStart = range.Target;
                (bool added2, long diff2) =
                    AddToRanges(ref ranges, ref referenceRanges, startRange, targetStart, rangeCount);
                if (added2)
                {
                    rangeToMapLeft -= diff2;
                    if (rangeToMapLeft < 0)
                    {
                        throw new Exception("RangeToMap has become negative");
                    }

                    if (ranges.Last().TargetEnd - ranges.Last().TargetStart < 0)
                    {
                        throw new Exception(
                            $"Wrong ending direct range {ranges.Last().TargetEnd - ranges.Last().TargetStart:n0} <=0");
                    }

                    Console.WriteLine(
                        $"{prefix} Right-Overlap {referenceRanges.Last()} {ranges.Last()} \tDiff {ranges.Last().TargetEnd - ranges.Last().TargetStart:n0}");
                }
            }

            // We've found the end of the splitrange
            break;
        }
        
        else
        {
            Console.WriteLine($"Full-Overlap (last {lastSourceRangeEnd}) {ranges.Last()}");
            {
                // FULL OVERLAP with direct-left, full range and the right part is either next iteration or post-loop administration
                var newStart = lastSourceRangeEnd + 1;
                var newEnd = startRange - 1;
                var rangeCountLo2 = newEnd - newStart + 1;
                if (rangeCountLo2 < 0)
                {
                    throw new Exception("Lo2 diff negative");
                }
                (bool added, long diff) =
                    AddToRanges(ref ranges, ref referenceRanges, newStart, newStart, rangeCountLo2);
                if (added)
                {
                    rangeToMapLeft -= diff;
                    if (rangeToMapLeft < 0)
                    {
                        throw new Exception("RangeToMap has become negative");
                    }

                    var prefixSpecial =
                        $"{link.Source}-{link.Target} Start {newStart:n0}\tEnd {newEnd:n0}";
                    Console.WriteLine(
                        $"{prefixSpecial} Direct-Map    {ranges.Last()} {ranges.Last()} \tDiff {ranges.Last().TargetEnd - ranges.Last().TargetStart:n0}");

                    if ((ranges.Last().TargetEnd - ranges.Last().TargetStart > 0) && ranges.Last().TargetEnd != newEnd)
                    {
                        throw new Exception($"Wrong ending direct range {ranges.Last().TargetEnd:n0} {newEnd:n0}");
                    }
                }
                else
                {
                    Console.WriteLine($"No direct mapping. Last end {lastSourceRangeEnd:n0}");
                }
            }

            {
                var rangeCount = range.Count;
                var targetStart = range.Target;
                (bool added, long diff) =
                    AddToRanges(ref ranges, ref referenceRanges, startRange, targetStart, rangeCount);
                if (added)
                {
                    rangeToMapLeft -= diff;
                    if (rangeToMapLeft < 0)
                    {
                        throw new Exception("RangeToMap has become negative");
                    }

                    Console.WriteLine(
                        $"{prefix} Full Overlap {referenceRanges.Last()} {ranges.Last()} \tDiff {ranges.Last().TargetEnd - ranges.Last().TargetStart:n0}");
                    lastSourceRangeEnd = endRange;
                }
                else
                {
                    throw new Exception("Unfinished FULL OVERLAP");
                }
            }
        }

        if (rangeToMapLeft < 0)
        {
            throw new Exception("RangeToMap has become negative");
        }
    }

    // Completely outside all ranges
    if (referenceRanges.Count == 0)
    {
        (bool added, long diff) =
            AddToRanges(ref ranges, ref referenceRanges, start, start, rangeToMapLeft);
        if (added)
        {
            rangeToMapLeft -= diff;
            if (rangeToMapLeft < 0)
            {
                throw new Exception("RangeToMap has become negative");
            }

            var prefixSpecial =
                $"{link.Source}-{link.Target} Start {start:n0}\tEnd {end:n0}";
            Console.WriteLine(
                $"{prefixSpecial} Direct-Map    {ranges.Last()} {ranges.Last()} \tDiff {ranges.Last().TargetEnd - ranges.Last().TargetStart:n0}");

            if (ranges.Last().TargetEnd != end)
            {
                throw new Exception($"Wrong ending direct range {ranges.Last().TargetEnd:n0} {end:n0}");
            }
        }
        else
        {
            throw new Exception("No overlap, must have direct mapping result");
        }
    }

    if (referenceRanges.Count == 0)
    {
        throw new Exception("No range mapped");
    }

    var checkDiff = referenceRanges.Last().TargetEnd - referenceRanges.First().TargetStart;
    var seedDiff = end - start;
    if (referenceRanges.Count >= 1)
    {
        if (checkDiff > seedDiff)
        {
            throw new Exception($"Mapped range end-start not equal to original end-start {checkDiff} {seedDiff}");
        }
    }

    if (checkDiff < seedDiff)
    {
        (bool added, long diff) =
            AddToRanges(ref ranges, ref referenceRanges, start, start, rangeToMapLeft);
        rangeToMapLeft -= diff;
        if (!added) throw new Exception("Direct map needed");
    }


    const int mustBe = 0;
    if (rangeToMapLeft != mustBe)
    {
        throw new Exception($"Range to map is not empty {rangeToMapLeft} != {mustBe}");
    }

    return (ranges, referenceRanges);
}

var minimum = long.MaxValue;
foreach (var (start, end) in seedCouples)
{
    Console.WriteLine($"Seed \t\t{start:n0}\tEnd {end:n0} \t\t\t\t\t\t\tCount {end - start + 1:n0}");

    List<(long TargetStart, long TargetEnd)> ranges = new()
    {
        (start, end)
    };
    List<(long Start, long End)> prevRanges = new();
    foreach (var transform in transformers)
    {
        Console.WriteLine($"\n+++ New Round - {ranges.Count} ranges to process");
        List<(long TargetStart, long TargetEnd)> newRanges = new();
        foreach (var range in ranges)
        {
            Console.WriteLine($"-- Processing Range ({range.TargetStart:n0}, {range.TargetEnd:n0})");
            var results = ProcessRange(transform, range.TargetStart, range.TargetEnd);
            // Console.WriteLine($"Received +{results.Ranges.Count - 1} ranges");
            newRanges.AddRange(results.Ranges);
            prevRanges.AddRange(results.ReferenceRanges);
        }

        prevRanges = ranges;
        ranges = newRanges;
        Console.WriteLine($"+++ Done {ranges.Count} ranges");
    }

    var minimumRange = ranges.Min(r => r.TargetStart);
    
    minimum = Math.Min(minimum, minimumRange);
    Console.WriteLine($"\nLowest location {minimumRange} {minimum}");
}

Console.WriteLine($"\nLowest location {minimum}");