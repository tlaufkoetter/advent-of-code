using System.Text.RegularExpressions;

namespace TwentyTwentyFour;

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
    private static IEnumerable<Point> GetAntinodes((Point, Point) pair)
    {
        var (pointA, pointB) = pair;
        var vector = new Point(pointB.X - pointA.X, pointB.Y - pointA.Y);
        yield return new Point(pointB.X + vector.X, pointB.Y + vector.Y);
        yield return new Point(pointA.X - vector.X, pointA.Y - vector.Y);

    }
    [Fact]
    public void Part1()
    {
        var antinodeCounts = GetMap()
            .GroupBy(antenna => antenna.Frequency, antenna => antenna.Coordinates)
            .SelectMany(points => points
                .SelectMany((pointA, i) => points.Skip(i + 1).Select(pointB => (pointA, pointB)))
                .SelectMany(GetAntinodes))
            .Distinct()
            .Where(point => point.X >= 0)
            .Where(point => point.Y >= 0)
            .Where(point => point.X < 50)
            .Where(point => point.Y < 50)
            .Count();
        Assert.Equal(336, antinodeCounts);
    }
}