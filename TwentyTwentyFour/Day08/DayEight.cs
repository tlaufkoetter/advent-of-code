using System.Text.RegularExpressions;
using TwentyTwentyFour.Utils;

namespace TwentyTwentyFour.Day08;

public class DayEight
{
    public record Antenna(char Frequency, Point Coordinates);
    private static Antenna[] GetMap()
    {
        return File.ReadAllLines("../../../Day08/DayEightInput.txt")
            .SelectMany((line, y) => line
                .Select((c, x) => new Antenna(c, new Point(x, y))))
                .Where(antenna => Regex.IsMatch($"{antenna.Frequency}", @"[a-zA-Z0-9]"))
            .ToArray();

    }
    private static IEnumerable<Point> Extrude(Point start, Point direction, int times)
    {
        return new Func<Point, Point>(previous => previous + direction)
            .AsRange(start, times)
            .Skip(1.CompareTo(times) + 1);

    }
    private static IEnumerable<Point> GetAntinodes((Point, Point) pair, int countPerDirection)
    {
        var (pointA, pointB) = pair;
        var vector = pointB - pointA;
        var extrude = (Point start, int flip = 1) => Extrude(start, vector / flip, countPerDirection);
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