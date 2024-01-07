using System.Diagnostics;

namespace q12;

public class Question
{
    public static int FindCombinations(List<char> springs, List<int> clusterSizes)
    {
        var groupCount = clusterSizes.Count;
        var separators = Math.Max(groupCount - 1, 0);
        var springCount = clusterSizes.Sum();
        var totalCount = separators + springCount;

        // Compile a minimum representation string of the cluster sizes
        var minRepr = string.Join('.', clusterSizes.Select(c => new string('#', c)));
        if (minRepr.Length != totalCount)
        {
            throw new UnreachableException();
        }

        // if (minRepr.Length == springs.Count)
        // {
        //     Console.WriteLine($"Options 1 Min {minRepr}");
        //     return 1;
        // }

        // Find groups of dots, which we can reduce to 1 dot
        var original = new string(springs.ToArray());
        var springString = new string(original);
        while (springString.Contains(".."))
        {
            springString = springString.Replace("..", ".");
        }

        // Edges dont matter
        springString = springString.Trim('.');

        var reducedDiff = springs.Count - springString.Length;
        var reprDiff = minRepr.Length - springString.Length;
        if (reprDiff == 0)
        {
            Console.WriteLine(
                $"Options 1 Min {minRepr} Simplified (len={springString.Length} del={reducedDiff})");
            return 1;
        }

        // More complex situation
        Console.WriteLine(
            $"Diff {reprDiff} Orig {original} Compare {springString} ({springString.Length}) with {minRepr} ({minRepr.Length})");

        // Next trick: no groups can be next to each other

        // Discover what ranges each group can fit in 
        // Split up the springs in dots
        var dotSeparatedGroups = SplitSubstringsWithIndices(original);

        // Track which place in the string the current cluster must minimally go
        var minimumIndex = 0;
        // Alternatively find out where each cluster can go
        foreach (var clusterSize in clusterSizes)
        {
            foreach (var (substring, index) in dotSeparatedGroups)
            {
                // if (rowSubstring.Length >=)
                //     Console.WriteLine(group);
            }
        }

        // Looking for combinations of how groups can fit in the unknown sections
        Console.WriteLine($"Options ? Min {minRepr}");
        return 1;
    }

    private static List<(string substring, int index)> SplitSubstringsWithIndices(string original)
    {
        var dotSeparatedGroups = new List<(string substring, int index)>();
        var splitGroups = new string(original).Split('.');

        int index = 0;
        foreach (var (springCluster, i) in splitGroups.Select((v, i) => (v, i)))
        {
            dotSeparatedGroups.Add((springCluster, index));
            if (i != splitGroups.Length - 1)
            {
                index += 1;
            }

            index += springCluster.Length;
        }

        return dotSeparatedGroups;
    }
}