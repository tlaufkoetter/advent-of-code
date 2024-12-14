using System.Text.RegularExpressions;

namespace TwentyTwentyFour;

public static class PointExtensions
{
    public static DayEight.Point Add(this DayEight.Point a, DayEight.Point b)
    {
        return new DayEight.Point(a.X + b.X, a.Y + b.Y);
    }

    public static DayEight.Point Sub(this DayEight.Point a, DayEight.Point b)
    {
        return new DayEight.Point(a.X - b.X, a.Y - b.Y);
    }

    public static DayEight.Point Div(this DayEight.Point a, int i)
    {
        return new DayEight.Point(a.X / i, a.Y / i);
    }
}

public class DayEight
{
    public record Point(int X, int Y);
    public record Antenna(char Frequency, Point Coordinates);
    private static Antenna[] GetMap()
    {
        return File.ReadAllLines("../../../DayEightInput.txt")
            .SelectMany((line, y) => line
                .Select((c, x) => new Antenna(c, new Point(x, y))))
                .Where(antenna => Regex.IsMatch($"{antenna.Frequency}", @"[a-zA-Z0-9]"))
            .ToArray();

    }
    private static IEnumerable<Point> Extrude(Point start, Point direction, int times)
    {
        return Enumerable.Range(0, times)
            .Aggregate(
                new List<Point>() { start }.AsEnumerable(),
                (agg, _) => agg.Append(agg.Last().Add(direction))).Skip(1.CompareTo(times) + 1);

    }
    private static IEnumerable<Point> GetAntinodes((Point, Point) pair, int countPerDirection)
    {
        var (pointA, pointB) = pair;
        var vector = pointB.Sub(pointA);
        var extrude = (Point start, int flip = 1) => Extrude(start, vector.Div(flip), countPerDirection);
        return extrude(pointB).Concat(extrude(pointA, -1));
    }

    private static int CountAntinodes(int countPerDirection)
    {
        return GetMap()
            .GroupBy(antenna => antenna.Frequency, antenna => antenna.Coordinates)
            .SelectMany(points => points
                .SelectMany((pointA, i) => points.Skip(i + 1).Select(pointB => (pointA, pointB)))
                .SelectMany(pair => GetAntinodes(pair, countPerDirection)))
            .Distinct()
            .Where(point => point.X >= 0)
            .Where(point => point.Y >= 0)
            .Where(point => point.X < 50)
            .Where(point => point.Y < 50)
            .Count();
    }
    [Fact]
    public void Part1()
    {
        Assert.Equal(336, CountAntinodes(1));
    }

    [Fact]
    public void Part2()
    {
        Assert.Equal(1131, CountAntinodes(32));
    }
}