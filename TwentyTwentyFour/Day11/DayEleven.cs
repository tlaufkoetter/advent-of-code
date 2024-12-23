using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using TwentyTwentyFour.Utils;
using Xunit.Sdk;

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
        if (stone == 0)
        {
            yield return 1;
        }
        else
        {
            var digits = (long)Math.Floor(Math.Log10(stone) + 1);
            if (digits % 2 == 0)
            {
                var den = (long)Math.Pow(10, digits / 2);
                yield return stone % den;
                yield return stone / den;
            }
            else
            {
                yield return stone * 2024;
            }
        }
    }

    [Theory]
    [InlineData(["../../../Day11/Example.txt", 55312])]
    [InlineData(["../../../Day11/Challenge.txt", 197357])]
    public void Part1(string file, long expected)
    {
        var count = GetCount(file, 25);
        Assert.Equal(expected, count);
    }

    public interface ITreeNode
    {
        long Count(int depth);
        long Transform(int blinks);
    }

    public class TreeRoot(IEnumerable<long> originalValues, Dictionary<long, TreeNode> cache) : ITreeNode
    {
        List<TreeNode> children = originalValues.Select(val => new TreeNode(val, cache)).ToList();
        public long Count(int depth)
        {
            return children.Sum(child => child.Count(depth));
        }

        public long Transform(int blinks)
        {
            return children.Sum(child => child.Transform(blinks));
        }
    }

    public class TreeNode(long originalValue, Dictionary<long, TreeNode> cache) : ITreeNode
    {
        private Dictionary<long, TreeNode> cache = cache;
        private long originalValue = originalValue;
        public long OriginalValue { get; } = originalValue;
        private List<TreeNode> children = [];

        public long Count(int depth)
        {
            if (depth == 0 || children.Count == 0)
            {
                return 1;
            }

            return children.Sum(child => child.Count(depth - 1));
        }

        private void AddChild(TreeNode node)
        {
            if (cache.TryAdd(node.OriginalValue, node))
            {
                children.Add(node);
            }
            else
            {
                children.Add(cache[node.OriginalValue]);
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
                return children.Sum(child => child.Transform(blinks - 1));
            }

            var digits = (long)Math.Floor(Math.Log10(OriginalValue) + 1);
            if (originalValue == 0)
            {
                AddChild(new TreeNode(1, cache));
            }
            else if (digits % 2 == 0)
            {
                var den = (long)Math.Pow(10, digits / 2);
                AddChild(new TreeNode(OriginalValue % den, cache));
                AddChild(new TreeNode(OriginalValue / den, cache));
            }
            else
            {
                AddChild(new TreeNode(OriginalValue * 2024, cache));
            }
            return Transform(blinks);
        }
    }

    private static long GetCount(string file, int blinks)
    {
        var input = GetInput(file);
        var stones = input.OrderDescending();
        var root = new TreeRoot(stones, []);
        return root.Transform(blinks);
    }

    [Theory]
    [InlineData(["../../../Day11/Example.txt", 55312])]
    [InlineData(["../../../Day11/Challenge.txt", 0])]
    public void Part2(string file, long expected)
    {
        var count = GetCount(file, 75);
        Assert.Equal(expected, count);
    }
}