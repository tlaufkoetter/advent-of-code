namespace TwentyTwentyFour;
public readonly struct Point(long x, long y)
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
}