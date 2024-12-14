using System.ComponentModel;

namespace TwentyTwentyFour;

public class DayOne
{
    static public (List<long>, List<long>) GetLists()
    {
        var lines = File.ReadAllLines("../../../DayOneInput.txt");
        var unsortedPairs = lines
            .Select(line => line.Split(' ', 2)
                .Select(part => part.Trim())
                .Select(long.Parse))
            .Select(parts => KeyValuePair.Create(parts.First(), parts.Last()))
            .ToDictionary();
        return ([.. unsortedPairs.Keys], [.. unsortedPairs.Values]);
    }
    static public long GetDistance()
    {
        var (left, right) = GetLists();
        return left.Order().Zip(right.Order())
            .Select(pair => pair.First - pair.Second)
            .Select(Math.Abs)
            .Sum();
    }

    static public long GetOtherDistance()
    {
        var (left, right) = GetLists();
        return left
             .Select(l => right.Where(r => r.Equals(l)).Sum())
             .Sum();
    }

    [Fact]
    public void Part1()
    {
        Assert.Equal(2769675, GetDistance());
    }

    [Fact]
    public void Part2()
    {
        Assert.Equal(24643097, GetOtherDistance());
    }
}
