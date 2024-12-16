namespace TwentyTwentyFour.Utils;
public readonly struct Point(long x, long y) : IComparable<Point>
{
    public long X { get; } = x;
    public long Y { get; } = y;

    public static Point operator +(Point a, Point b)
    {
        return new Point(a.X + b.X, a.Y + b.Y);
    }

    public static Point operator -(Point a, Point b)
    {
        return new Point(a.X - b.X, a.Y - b.Y);
    }

    public static Point operator /(Point a, long i)
    {
        return new Point(a.X / i, a.Y / i);
    }

    public static Point operator *(Point a, long i)
    {
        return new Point(a.X * i, a.Y * i);
    }

    public static Point operator %(Point a, Point b)
    {
        return new Point(a.X % b.X, a.Y % b.Y);
    }

    public override string ToString()
    {
        return $"({X},{Y})";
    }

    public int CompareTo(Point other)
    {
        return new int[] { X.CompareTo(other.X), Y.CompareTo(other.Y) }.Where(c => c != 0).Order().FirstOrDefault(0);
    }
}

public record WidePoint(long[] Xs, long Y)
{
    public static WidePoint operator +(WidePoint a, Point b)
    {
        return new WidePoint(a.Xs.Select(x => x + b.X).ToArray(), a.Y + b.Y);
    }

    public static WidePoint operator -(WidePoint a, WidePoint b)
    {
        return new WidePoint(a.Xs.Zip(b.Xs).Select(p => p.First - p.Second).ToArray(), a.Y - b.Y);
    }

    public override string ToString()
    {
        return $"(({string.Join(',', Xs)}),{Y})";
    }
}

public static class WidePointExtensions
{
    public static IEnumerable<Point> AsSinglePoints(this WidePoint point)
    {
        return point.Xs.Select(x => new Point(x, point.Y));
    }

    public static bool WideContains(this IEnumerable<WidePoint> source, WidePoint value)
    {
        var points = source.SelectMany(point => point.AsSinglePoints()).ToHashSet();
        return points.Intersect(value.AsSinglePoints().ToHashSet()).Any();
    }

    public static IEnumerable<WidePoint> WideExcept(this IEnumerable<WidePoint> source, IEnumerable<WidePoint> values)
    {
        var points = values.SelectMany(point => point.AsSinglePoints()).ToHashSet();
        return source.Where(point => point.AsSinglePoints().All(p => !points.Contains(p)));
    }
}
