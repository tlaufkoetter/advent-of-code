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

    public override string ToString()
    {
        return $"(({string.Join(',', Xs)}),{Y})";
    }
}