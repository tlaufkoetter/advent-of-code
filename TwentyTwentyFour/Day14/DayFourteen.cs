using System.Text.RegularExpressions;
using TwentyTwentyFour.Utils;

namespace TwentyTwentyFour.Day14;

public record Robot(Point Position, Point Velocity);

public class DayFourteen
{
    private static long Parse(Match match, int index)
    {
        return long.Parse(match.Groups[index].Value);
    }
    static readonly Point map = new Point(101, 103);

    public static IEnumerable<Robot> GetRobots()
    {
        return File.ReadAllLines("../../../Day14/DayFourteenInput.txt")
            .Select(line => Regex.Match(line, @"p=(\d+),(\d+) v=(-?\d+),(-?\d+)"))
            .Select(match => new Robot(new Point(Parse(match, 1), Parse(match, 2)), new Point(Parse(match, 3), Parse(match, 4))));
    }

    public static IEnumerable<Robot> Tick(IEnumerable<Robot> robots)
    {
        return robots
            .Select(robot => robot with { Position = (robot.Position + robot.Velocity + map) % map });
    }

    public static long CalculateSafetyFactor(IEnumerable<Robot> robots)
    {
        var xLine = map.X / 2;
        var yLine = map.Y / 2;
        return robots
            .Select(robot => robot.Position)
            .GroupBy(position => Math.Abs(position.X.CompareTo(xLine) * position.Y.CompareTo(yLine))
                * (1
                    | position.X.CompareTo(xLine).CompareTo(-1) << 1
                    | position.Y.CompareTo(yLine).CompareTo(-1) << 2))
            .OrderByDescending(group => group.Key)
            .Take(4)
            .Aggregate(1, (agg, group) => agg * group.Count());
    }


    [Fact]
    public void Part1()
    {
        Assert.Equal(217328832, CalculateSafetyFactor(Enumerable.Range(0, 100).Aggregate(GetRobots(), (agg, _) => Tick(agg))));
    }
}