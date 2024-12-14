using System.Text.RegularExpressions;

namespace TwentyTwentyFour;

public record Machine(Point ButtonA, Point ButtonB, Point Prize);

public class DayThirteen
{

    IEnumerable<Machine> GetMachines()
    {
        return File.ReadAllText("../../../DayThirteenInput.txt")
            .Split("\n\n")
            .Select(chunk =>
            {
                var points = Regex.Matches(chunk, @"X.(\d+), Y.(\d+)").Select(match => new Point(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value))).ToArray();
                return new Machine(points[0], points[1], points[2]);
            });
    }

    [Fact]
    public void Day1()
    {
        var coins = GetMachines().SelectMany(machine =>
            Enumerable.Range(0, 101).Select(timesB => Enumerable.Range(0, 101)
                .Select(timesA => (machine.ButtonB.Mul(timesB).Add(machine.ButtonA.Mul(timesA)), timesB + 3 * timesA))
                .Where(pair => pair.Item1.Equals(machine.Prize))
                .Select(pair => pair.Item2)
                .Order()
                .Append(0)
                .First())
        ).Sum();
        Assert.Equal(33209, coins);
    }
}