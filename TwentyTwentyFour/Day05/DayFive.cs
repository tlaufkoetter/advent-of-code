using System.Collections.Immutable;
using System.Data;

namespace TwentyTwentyFour.Day05;

public class DayFive
{
    private static (HashSet<(int, int)>, int[][]) GetInput(string file)
    {

        var parts = File.ReadAllText(file)
            .Split("\n\n");
        return (
            parts[0]
                .Split('\n')
                .Select(line => line
                    .Split('|')
                    .Select(int.Parse)
                    .ToArray())
                .Select(split => (split[0], split[1]))
                .ToHashSet(),
            parts[1]
                .Split('\n')
                .Select(line => line.Split(',')
                    .Select(int.Parse)
                    .ToArray())
                .ToArray()
        );
    }

    private static (HashSet<(int, int)>, int[][]) GetExampleInput() => GetInput("../../../Day05/DayFiveExample.txt");
    private static (HashSet<(int, int)>, int[][]) GetChallengeInput() => GetInput("../../../Day05/DayFiveInput.txt");

    private static bool IsInvalid(HashSet<(int, int)> rules, int[] update) => !IsValid(rules, update);
    private static bool IsValid(HashSet<(int, int)> rules, int[] update)
    {
        return !update
            .SkipLast(1)
            .SelectMany((before, i) => update
                .Skip(i + 1)
                .Select(after => (after, before)))
            .Any(rules.Contains);
    }

    private static long CalculateSum(IEnumerable<int[]> updates)
    {
        return updates.Select(update => update.ElementAt(update.Length / 2)).Sum();
    }

    private static IEnumerable<int[]> GetValidUpdates(HashSet<(int, int)> rules, IEnumerable<int[]> updates)
    {
        return updates.Where(update => IsValid(rules, update));
    }

    [Fact]
    public void Part1()
    {
        var (rulesEx, updatesEx) = GetExampleInput();
        Assert.Equal(143, CalculateSum(GetValidUpdates(rulesEx, updatesEx)));
        var (rules, updates) = GetChallengeInput();
        Assert.Equal(5091, CalculateSum(GetValidUpdates(rules, updates)));
    }

    private static IEnumerable<int> Sort(HashSet<(int, int)> rules, List<int> update)
    {
        var first = update.Where((before, i) => update.Skip(i + 1).All(after => !rules.Contains((after, before)))).First();
        return new List<int> { first }.Concat(new Func<List<int>>[] { () => [], () => Sort(rules, update.Where(page => page != first).ToList()).ToList() }.Skip(update.Count.CompareTo(1)).First()());
    }

    private static IEnumerable<int[]> GetInvalidSorted(HashSet<(int, int)> rules, IEnumerable<int[]> updates)
    {
        var result = updates.Where(update => IsInvalid(rules, update)).Select(update => Sort(rules, [.. update]).ToArray()).ToArray();
        return result;
    }

    [Fact]
    public void Part2()
    {
        var (rulesEx, updatesEx) = GetExampleInput();
        Assert.Equal(123, CalculateSum(GetInvalidSorted(rulesEx, updatesEx)));
        var (rules, updates) = GetChallengeInput();
        Assert.Equal(4681, CalculateSum(GetInvalidSorted(rules, updates)));
    }
}