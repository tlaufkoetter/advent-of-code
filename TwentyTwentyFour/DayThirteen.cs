using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace TwentyTwentyFour;

public record Machine(Point ButtonA, Point ButtonB, Point Prize);

public class DayThirteen
{
    private static long Parse(Match match, int index)
    {
        return long.Parse(match.Groups[index].Value);
    }

    static IEnumerable<Machine> GetMachines()
    {
        return File.ReadAllText("../../../DayThirteenInput.txt")
            .Split("\n\n")
            .Select(chunk =>
            {
                var points = Regex.Matches(chunk, @"X.(\d+), Y.(\d+)")
                    .Select(match => new Point(Parse(match, 1), Parse(match, 2)))
                    .ToArray();
                return new Machine(points[0], points[1], points[2]);
            });
    }

    public static long SumLeastAmountOfCoins(IEnumerable<Machine> machines, long limit = 100)
    {
        return machines.Select(machine =>
        {
            var buttonA = machine.ButtonA;
            var buttonB = machine.ButtonB;
            var prize = machine.Prize;
            var mA = new Fraction(buttonA.Y, buttonA.X);
            var mB = new Fraction(buttonB.Y, buttonB.X);
            var bA = new Fraction(prize.Y) - new Fraction(prize.X) * mA;
            var intX = bA / (mB - mA);
            var intY = intX * mB;
            var timesXB = intX / new Fraction(buttonB.X);
            var timesYB = intY / new Fraction(buttonB.Y);
            var timesB = Math.Min(timesXB.ToNatural(), timesYB.ToNatural());
            var timesXA = (new Fraction(prize.X) - intX) / new Fraction(buttonA.X);
            var timesYA = (new Fraction(prize.Y) - intY) / new Fraction(buttonA.Y);
            var timesA = Math.Min(timesXA.ToNatural(), timesYA.ToNatural());
            return new { Machine = machine, TimesA = timesA, TimesB = timesB };
        })
        .Where(times => times.TimesA >= 0)
        .Where(times => times.TimesB >= 0)
        .Where(times => times.TimesA + times.TimesB > 0)
        .Where(times => times.TimesA <= limit)
        .Where(times => times.TimesB <= limit)
        .Where(times => (times.Machine.ButtonA * times.TimesA + times.Machine.ButtonB * times.TimesB)
            .Equals(times.Machine.Prize))
        .Select(times => times.TimesA * 3 + times.TimesB)
        .Sum();

    }

    [Fact]
    public void Day1()
    {
        Assert.Equal(33209, SumLeastAmountOfCoins(GetMachines()));
    }

    [Fact]
    public void Day2()
    {
        Assert.Equal(83102355665474, SumLeastAmountOfCoins(GetMachines().Select(machine => machine with { Prize = new Point(machine.Prize.X + 10000000000000, machine.Prize.Y + 10000000000000) }), long.MaxValue));
    }
}