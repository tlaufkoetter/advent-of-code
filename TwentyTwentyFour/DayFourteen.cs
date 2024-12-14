using System.Text.RegularExpressions;

namespace TwentyTwentyFour;

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
        return File.ReadAllLines("../../../DayFourteenInput.txt")
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
        var snapshot = robots.ToList();
        var xLine = map.X / 2;
        var yLine = map.Y / 2;
        return snapshot.Where(robot => robot.Position.X < xLine).Where(robot => robot.Position.Y < yLine).Count()
            * snapshot.Where(robot => robot.Position.X > xLine).Where(robot => robot.Position.Y < yLine).Count()
            * snapshot.Where(robot => robot.Position.X < xLine).Where(robot => robot.Position.Y > yLine).Count()
            * snapshot.Where(robot => robot.Position.X > xLine).Where(robot => robot.Position.Y > yLine).Count();
    }


    [Fact]
    public void Part1()
    {
        Assert.Equal(217328832, CalculateSafetyFactor(Enumerable.Range(0, 100).Aggregate(GetRobots(), (agg, _) => Tick(agg))));
    }
}