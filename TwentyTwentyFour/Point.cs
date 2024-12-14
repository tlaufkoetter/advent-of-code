namespace TwentyTwentyFour;
public record Point(int X, int Y);
public static class PointExtensions
{
    public static Point Add(this Point a, Point b)
    {
        return new Point(a.X + b.X, a.Y + b.Y);
    }

    public static Point Sub(this Point a, Point b)
    {
        return new Point(a.X - b.X, a.Y - b.Y);
    }

    public static Point Div(this Point a, int i)
    {
        return new Point(a.X / i, a.Y / i);
    }

    public static Point Mul(this Point a, int i)
    {
        return new Point(a.X * i, a.Y * i);
    }
}