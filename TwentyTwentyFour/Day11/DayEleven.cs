namespace TwentyTwentyFour.Day11;

public class TreeRoot(IEnumerable<long> originalValues, Dictionary<long, TreeNode> cache)
{
    private readonly List<TreeNode> children = originalValues.Select(val => new TreeNode(val, cache)).ToList();
    public long Transform(int blinks)
    {
        return children.Sum(child => child.Transform(blinks));
    }
}

public class TreeNode
{
    public TreeNode(long originalValue, Dictionary<long, TreeNode> cache)
    {
        this.cache = cache;
        this.originalValue = originalValue;
        cache[originalValue] = this;
    }
    private readonly Dictionary<long, TreeNode> cache;

    private readonly long originalValue;
    private readonly List<TreeNode> children = [];

    private readonly Dictionary<int, long> countCache = [];

    private void AddChildren(IEnumerable<long> values)
    {
        Func<long, TreeNode>[] results = [
            value => new TreeNode(value, cache),
            value => cache[value]
        ];
        children.AddRange(values
            .Select(value => results[Convert.ToInt32(cache.ContainsKey(value))](value)));
    }

    public long Transform(int blinks)
    {
        Func<long>[] results = [
            () => 1L,
            () => {
                Func<long>[] results = [
                    () => children.Count,
                    () => countCache[blinks],
                    () => {
                        var childrenCount = children.Sum(child => child.Transform(blinks - 1));
                        countCache[blinks] = childrenCount;
                        return childrenCount;
                    }
                ];
                var index = blinks.CompareTo(1);
                index *= index + (1 - Convert.ToInt32(countCache.ContainsKey(blinks)));
                return results[index]();
            },
            () => {
                var digits = (long)Math.Floor(Math.Log10(originalValue) + 1);
                var index = originalValue.CompareTo(0);
                index *= index + (1 - Convert.ToInt32(digits % 2 == 0));
                Func<long[]>[] results = [
                    () => [1],
                    () => {
                        var den = (long)Math.Pow(10, digits / 2);
                        return [originalValue % den, originalValue / den];
                    },
                    () => [originalValue * 2024]
                ];
                AddChildren(results[index]());
                return Transform(blinks);
            }
        ];
        return results[1 - children.Count.CompareTo(0) + blinks.CompareTo(0)]();
    }
}

public class DayEleven
{
    private static List<long> GetInput(string file)
    {
        return File.ReadAllText(file).Trim()
            .Split(' ')
            .Select(long.Parse)
            .ToList();
    }

    private static long GetCount(string file, int blinks)
    {
        return new TreeRoot(GetInput(file), []).Transform(blinks);
    }

    [Theory]
    [InlineData(["../../../Day11/Example.txt", 55312])]
    [InlineData(["../../../Day11/Challenge.txt", 197357])]
    public void Part1(string file, long expected)
    {
        var count = GetCount(file, 25);
        Assert.Equal(expected, count);
    }

    [Theory]
    [InlineData(["../../../Day11/Challenge.txt", 234568186890978])]
    public void Part2(string file, long expected)
    {
        var count = GetCount(file, 75);
        Assert.Equal(expected, count);
    }
}