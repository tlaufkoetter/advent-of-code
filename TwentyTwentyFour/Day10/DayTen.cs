namespace TwentyTwentyFour.Day10;

using TwentyTwentyFour.Utils;

public class DayTen
{
    public record HikingPoint(Point Position, int Value);
    private static IDictionary<Point, int> GetInput(string file)
    {
        return File.ReadAllLines(file)
            .SelectMany((line, y) => line.Select((value, x) => KeyValuePair.Create(new Point(x, y), value - 48)))
            .ToDictionary();
    }

    [Theory]
    [InlineData(["../../../Day10/Example.txt", 36])]
    [InlineData(["../../../Day10/Challenge.txt", 593])]
    public void Part1(string filePath, long expected)
    {
        var input = GetInput(filePath);
        List<Point> ways = [new(0, 1), new(0, -1), new(1, 0), new(-1, 0)];
        var max = input.Keys.Max();
        var min = input.Keys.Min();
        var trails = Enumerable.Range(1, 9)
            .Aggregate(input.Where(kvp => kvp.Value == 0).Select(s => new List<Point> { s.Key }), (currentTrails, next) =>
                currentTrails
                    .SelectMany(current => ways
                        .Select(way => current.Last() + way)
                        .Where(p => p.X <= max.X)
                        .Where(p => p.Y <= max.Y)
                        .Where(p => p.X >= min.X)
                        .Where(p => p.Y >= min.Y)
                        .Select(p => new HikingPoint(p, input[p]))
                        .Where(p => p.Value == next)
                        .Select(nextP => current.Append(nextP.Position).ToList()))
                    .DistinctBy(current => (current.First(), current.Last()))
            )
            .Select(look => look.First())
            .ToList();
        Assert.Equal(expected, trails.Count);
    }
}