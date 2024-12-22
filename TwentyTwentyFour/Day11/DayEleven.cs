using TwentyTwentyFour.Utils;

namespace TwentyTwentyFour.Day11;

public class DayEleven
{
    private static List<long> GetInput(string file)
    {
        return File.ReadAllText(file).Trim()
            .Split(' ')
            .Select(long.Parse)
            .ToList();
    }

    private static IEnumerable<long> TransformStone(long stone)
    {
        var stoneIsZero = Enumerable.Range(0, (int)Math.Max(0L, 1 - stone))
            .Select<int, IEnumerable<long>>(_ => [stone + 1]);
        var stoneDigitsEven = Enumerable.Range(0, (int)Math.Min(1L, stone))
            .Select(_ => stone.ToString())
            .Where(s => s.Length % 2 == 0)
            .Select(s => new List<string>() { s[..(s.Length / 2)], s[(s.Length / 2)..] }
                .Select(s => long.Parse(s)));
        IEnumerable<IEnumerable<long>> choices = [.. stoneIsZero, .. stoneDigitsEven, [stone * 2024]];
        return choices.First();
    }

    [Theory]
    [InlineData(["../../../Day11/Example.txt", 55312])]
    [InlineData(["../../../Day11/Challenge.txt", 197357])]
    public void Part1(string file, long expected)
    {
        var count = GetCount(file, 25);
        Assert.Equal(expected, count);

    }

    private static int GetCount(string file, int blinks)
    {
        return new Func<IEnumerable<long>, IEnumerable<long>>(agg =>
                        agg.SelectMany(TransformStone)
                    ).AsRange(GetInput(file), blinks)
                    .Last().Count();
    }

    [Theory]
    [InlineData(["../../../Day11/Challenge.txt", 0])]
    public void Part2(string file, long expected)
    {
        var count = GetCount(file, 75);
        Assert.Equal(expected, count);
    }
}