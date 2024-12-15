namespace TwentyTwentyFour;

public readonly struct Fraction
{
    public readonly long num;
    public readonly long den;

    private static long Gcd(long numerator, long denominator)
    {
        var ordered = new[] { numerator, denominator }.OrderDescending().ToArray();
        var rest = ordered[0] % ordered[1];
        return ordered
            .TakeLast(1)
            .Select<long, Func<long>>(n => () => n)
            .Append(() => Gcd(ordered.Last(), rest))
            .Skip(Math.Abs(rest.CompareTo(0)))
            .Take(1).First()();
    }

    public Fraction(long numerator, long denominator)
    {
        var gcd = Gcd(Math.Abs(numerator), Math.Abs(denominator));
        num = numerator / gcd;
        den = denominator / gcd;
    }

    public Fraction(long numerator) : this(numerator, 1)
    {
    }

    public static Fraction operator +(Fraction a) => a;
    public static Fraction operator -(Fraction a) => new Fraction(-a.num, a.den);

    public static Fraction operator +(Fraction a, Fraction b)
        => new(a.num * b.den + b.num * a.den, a.den * b.den);

    public static Fraction operator -(Fraction a, Fraction b)
        => a + (-b);

    public static Fraction operator *(Fraction a, Fraction b)
        => new(a.num * b.num, a.den * b.den);

    public static Fraction operator /(Fraction a, Fraction b)
    {
        return new(a.num * b.den, a.den * b.num);
    }

    public long ToNatural()
    {
        return num / den;
    }
}
