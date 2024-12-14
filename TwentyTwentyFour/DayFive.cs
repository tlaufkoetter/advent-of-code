namespace TwentyTwentyFour;

public class DayFive
{
    private static (HashSet<(int, int)>, int[][]) GetInput()
    {
        var parts = File.ReadAllText("../../../DayFiveInput.txt")
            .Split("\n\n");
        return
        (
            parts[0]
                .Split('\n')
                .Select(line => line
                    .Split('|')
                    .Select(int.Parse)
                    .ToArray())
                .Select(split => (split[0], split[1]))
                .ToHashSet(),
            parts[1]
                .Split('\n')
                .Select(line => line.Split(',')
                    .Select(int.Parse)
                    .ToArray())
                .ToArray()
        );
    }

    private static bool IsValid(HashSet<(int, int)> rules, int[] update)
    {
        return !update
            .SkipLast(1)
            .SelectMany((before, i) => update
                .Skip(i + 1)
                .Select(after => (after, before)))
            .Any(rules.Contains);
    }

    [Fact]
    public void Part1()
    {
        var (rules, printOrders) = GetInput();
        Assert.Equal(5091, printOrders.Where(order => IsValid(rules, order)).Select(order => order.ElementAt(order.Length / 2)).Sum());
    }
}