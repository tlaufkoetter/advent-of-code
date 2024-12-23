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

    private void AddChild(long value)
    {
        if (cache.TryGetValue(value, out var node))
        {
            children.Add(node);
        }
        else
        {
            children.Add(new TreeNode(value, cache));
        }
    }

    public long Transform(int blinks)
    {
        if (blinks == 0)
        {
            return 1;
        }

        if (children.Count > 0)
        {
            if (blinks == 1)
            {
                return children.Count;
            }

            if (countCache.TryGetValue(blinks, out var cachedCount))
            {
                return cachedCount;
            }

            var childrenCount = children.Sum(child => child.Transform(blinks - 1));
            countCache[blinks] = childrenCount;
            return childrenCount;
        }

        var digits = (long)Math.Floor(Math.Log10(originalValue) + 1);
        if (originalValue == 0)
        {
            AddChild(1);
        }
        else if (digits % 2 == 0)
        {
            var den = (long)Math.Pow(10, digits / 2);
            AddChild(originalValue % den);
            AddChild(originalValue / den);
        }
        else
        {
            AddChild(originalValue * 2024);
        }
        return Transform(blinks);
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