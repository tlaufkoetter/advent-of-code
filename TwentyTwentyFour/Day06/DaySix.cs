using TwentyTwentyFour.Utils;

namespace TwentyTwentyFour.Day06;

public static class Extensions
{
    public static IEnumerable<TSource> TakeWhileIncluding<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        var enumerator = source.GetEnumerator();
        var whytho = Enumerable.Repeat(false, int.MaxValue)
            .TakeWhile(_ => enumerator.MoveNext())
            .Select(_ => enumerator.Current)
            .TakeWhile(predicate);

        return whytho.Concat(Enumerable.Repeat(false, 1)
            .TakeWhile(_ => enumerator.MoveNext())
            .Select(_ => enumerator.Current));
    }
}

public class DaySix
{
    public record InputData(Guard Guard, HashSet<Point> Obstacles, Point MapDimensions);
    public record Guard(Point Position, Point Direction) : IComparable<Guard>
    {
        public int CompareTo(Guard? other)
        {
            return new int[] { Position.CompareTo(other!.Position), Direction.CompareTo(other.Direction) }.MaxBy(Math.Abs);
        }

        public Guard Move()
        {
            return this with { Position = Position + Direction };
        }

        public Guard Rotate()
        {
            return this with { Direction = new Point(-Direction.Y, Direction.X) };
        }
    }

    private static InputData GetInput(string file)
    {
        var dict = File.ReadAllLines(file)
            .SelectMany((row, y) => row.Select((cell, x) => (cell, new Point(x, y))))
            .GroupBy(cell => cell.cell, cell => cell.Item2)
            .ToDictionary(group => group.Key, group => group.AsEnumerable());
        return new InputData(new Guard(dict['^'].First(), new Point(0, -1)), dict['#'].ToHashSet(), dict.Values.SelectMany(p => p).Max());
    }

    [Theory]
    [InlineData(["../../../Day06/DaySixExample.txt", 41])]
    [InlineData(["../../../Day06/DaySixInput.txt", 5080])]
    public void Part1(string inputFile, int expected)
    {
        Assert.Equal(expected, GetPositions(GetInput(inputFile)).Select(point => point.Position).Distinct().Count());
    }

    [Theory]
    [InlineData(["../../../Day06/DaySixExample.txt", 6])]
    [InlineData(["../../../Day06/DaySixInput.txt", 1919])]
    public void Part2(string inputFile, int expected)
    {
        var input = GetInput(inputFile);
        var originalPath = GetPositions(input).Where(pos => !input.Guard.Position.Equals(pos.Position)).Distinct().ToList();
        var circularPaths = originalPath
            .Select(guard => guard.Position)
            .Distinct()
            .Select(extraObstacle => GetPositions(input with { Obstacles = [.. input.Obstacles, extraObstacle] })
                .ToList())
            .Where(path => path.Count > path.Distinct().Count())
            .Select(p => p.ToHashSet())
            .Distinct(HashSet<Guard>.CreateSetComparer());
        Assert.Equal(expected, circularPaths.Count());
    }

    private static IEnumerable<Guard> GetPositions(InputData input)
    {
        HashSet<Guard> visited = [];
        var guard = input.Guard;
        var mapDimensions = input.MapDimensions;
        return new Func<Guard, Guard>(currentGuard =>
                new Guard[]  {
                    currentGuard.Move(),
                    currentGuard.Rotate(),
                }[Convert.ToInt16(input.Obstacles.Contains(currentGuard.Move().Position))])
            .AsRange(guard, (int)(input.MapDimensions.X * input.MapDimensions.Y))
            .TakeWhile(guard => Math.Min(1, Math.Max(0, 1 + Math.Min(guard.Position.CompareTo(new Point(0, 0)), mapDimensions.CompareTo(guard.Position)))) == 1)
            .TakeWhileIncluding(visited.Add);
    }
}