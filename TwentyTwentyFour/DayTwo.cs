using System.Windows.Markup;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace TwentyTwentyFour;

public class DayTwo
{
    public static long[][] GetLines()
    {
        return File.ReadAllLines("../../../DayTwoInput.txt")
            .Select(line => line.Split(' ').Select(long.Parse).ToArray())
            .ToArray();
    }

    public static bool IsSafe(long[] values, Func<long[], bool>[] predicates)
    {
        return predicates.All(prediacte => prediacte(values));
    }

    public static bool IsAscending(long[] values)
    {
        return values.SequenceEqual(values.Order());
    }

    public static bool IsDescending(long[] values)
    {
        return values.SequenceEqual(values.OrderDescending());
    }

    public static bool IsMonotonous(long[] values)
    {
        Func<long[], bool>[] directions = [IsAscending, IsDescending];
        var result = directions.Any(direction => direction(values));
        return result;
    }

    public static bool IsCorrectSlope(long[] values)
    {
        var diffs = values.SkipLast(1).Zip(values.Skip(1)).Select(p => p.First - p.Second).Select(Math.Abs).ToArray();
        Func<long[], bool>[] bounds = [vals => vals.All(v => v <= 3), vals => vals.All(v => v >= 1)];
        var result = bounds.All(bound => bound(diffs));
        return result;
    }

    [Fact]
    public void Part1()
    {
        var lines = GetLines();
        Assert.Equal(332, lines.Where(line => IsSafe(line, [IsMonotonous, IsCorrectSlope])).Count());
    }
}
