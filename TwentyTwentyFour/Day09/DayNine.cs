using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using TwentyTwentyFour.Utils;

namespace TwentyTwentyFour.Day09
{

    record BlockPair(long Data, long Free);
    public class DayNine
    {

        private static BlockPair[] GetInput(string file)
        {
            var text = File.ReadAllText(file).Trim();
            var data = Enumerable.Range(0, (text.Length + 1) / 2)
                .Select(i => i * 2)
                .Select(i => text[i]);
            var free = Enumerable.Range(1, (text.Length + 1) / 2 - 1)
                .Select(i => i * 2 - 1)
                .Select(i => text[i])
                .Append('0');
            return data.Zip(free).Select(p => new BlockPair(p.First - 48, p.Second - 48)).ToArray();
        }

        [Fact]
        public void GetInputTest()
        {
            var actual = GetInput("../../../Day09/Example.txt");
            Assert.Equal([new(2, 3), new(3, 3), new(1, 3), new(3, 1), new(2, 1), new(4, 1), new(4, 1), new(3, 1), new(4, 0), new(2, 0)],
            actual);
        }

        private static long CalculateCheckSum(IEnumerable<long?> indices)
        {
            return indices.Select((n, i) => (n ?? 0) * i).Sum()!;
        }

        [Theory]
        [InlineData(1928L, 0L, 0L, 9L, 9L, 8L, 1L, 1L, 1L, 8L, 8L, 8L, 2L, 7L, 7L, 7L, 3L, 3L, 3L, 6L, 4L, 4L, 6L, 5L, 5L, 5L, 5L, 6L, 6L)]
        [InlineData(2858L, 0L, 0L, 9L, 9L, 2L, 1L, 1L, 1L, 7L, 7L, 7L, null, 4L, 4L, null, 3L, 3L, 3L, null, null, null, null, 5L, 5L, 5L, 5L, null, 6L, 6L, 6L, 6L, null, null, null, null, null, 8L, 8L, 8L, 8L, null, null)]
        public void TestCheckSum(long expected, params long?[] array)
        {
            Assert.Equal(expected, CalculateCheckSum(array));
        }

        private static long?[] DefragArray(long?[] data)
        {
            var defragged = data.Select((n, i) => (n, i))
                .Where(p => p.n == null)
                .Select(p => p.i)
                .Zip(data.Select((n, i) => (n, i)).Where(p => p.n != null).Reverse())
                .Aggregate(new List<long?>(),
                    (agg, pair) =>
                    [
                        .. agg,
                        .. data.Skip(agg.Count).Take(pair.First - agg.Count)
                        .Append(pair.Second.n)
                        .TakeWhile(_ => pair.Second.i > pair.First),
                    ]).ToList();
            return [.. defragged, .. data.Skip(defragged.Count).Take(data.Where(n => n != null).Count() - defragged.Count)];
        }

        private static List<List<int>> FindEmptySpaces(long?[] data)
        {
            return data.Select((n, i) => (n, i))
                            .Where(p => p.n == null)
                            .Select(p => p.i)
                            .Aggregate(new List<List<int>>() { new() { -2 } }, (agg, i) =>
                                agg
                                    .SkipLast(1)
                                    .Concat(agg
                                        .TakeLast(1)
                                        .Select(last => last
                                            .Concat(last
                                                .TakeLast(1)
                                                .Where(l => l == i - 1)
                                                .Select(_ => i)).ToList()))
                                    .Concat(agg
                                        .TakeLast(1)
                                        .Where(last => !last
                                            .Contains(i - 1))
                                        .Select(_ => new List<int> { i })
                                    ).ToList())
                            .Skip(1).ToList();
        }


        private static long?[] DefragArrayWholeFiles(long?[] data)
        {
            var fileSpaces = data.Select((n, i) => (n, i))
                .Where(p => p.n != null)
                .Aggregate(new List<List<(long, int)>>() { new() { (0, -2) } }, (agg, pair) =>
                    agg
                        .SkipLast(1)
                        .Concat(agg
                            .TakeLast(1)
                            .Select(last => last
                                .Concat(last
                                    .TakeLast(1)
                                    .Where(l => l.Item2 == pair.i - 1 && l.Item1 == pair.n)
                                    .Select(_ => (pair.n ?? 0, pair.i)))
                                    .ToList())
                            .ToList())
                        .Concat(agg
                            .TakeLast(1)
                            .Where(last => !last
                                .Contains((pair.n ?? 0, pair.i - 1)))
                            .Select(_ => new List<(long, int)> { (pair.n ?? 0, pair.i) })
                        ).ToList())
                .Skip(1).ToList();
            return fileSpaces.AsEnumerable()
                .Reverse()
                .AggregateList(new { Data = data, EmptySpaces = FindEmptySpaces(data) }, (agg, space) =>
                {
                    var freeSpace = agg.EmptySpaces.Select((sp, i) => new { Index = i, Spaces = sp }).Where(es => es.Spaces.Count >= space.Count).Where(s => s.Spaces[0] < space[0].Item2).Take(1).ToList();
                    freeSpace.SelectMany(_ => space.Select(s => (long?)s.Item1)).ToArray().CopyTo(agg.Data, freeSpace.FirstOrDefault()?.Spaces[0] ?? 0);
                    freeSpace.SelectMany(_ => space).Select(s => (long?)null).ToArray().CopyTo(agg.Data, space.FirstOrDefault().Item2);
                    var newEmptySpaces = freeSpace
                        .Select(fs => fs with { Spaces = fs.Spaces.Skip(space.Count).ToList() })
                        .Select(fs => agg
                            .EmptySpaces
                            .Take(fs.Index)
                            .Concat(fs.Spaces
                                .Select(_ => fs.Spaces).Take(1))
                            .Concat(agg.EmptySpaces.Skip(fs.Index + 1)).ToList())
                        .Append(agg.EmptySpaces)
                        .First();
                    return agg with { EmptySpaces = newEmptySpaces };
                })
                .Last().Data;

        }

        [Fact]
        public void DefragArrayWholeFilesTest()
        {
            long?[] expected = [0, 0, 9, 9, 2, 1, 1, 1, 7, 7, 7, null, 4, 4, null, 3, 3, 3, null, null, null, null, 5, 5, 5, 5, null, 6, 6, 6, 6, null, null, null, null, null, 8, 8, 8, 8, null, null];
            var result = DefragArrayWholeFiles([0, 0, null, null, null, 1, 1, 1, null, null, null, 2, null, null, null, 3, 3, 3, null, 4, 4, null, 5, 5, 5, 5, null, 6, 6, 6, 6, null, 7, 7, 7, null, 8, 8, 8, 8, 9, 9]);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void DefragTest()
        {
            var result = DefragArray([0, 0, null, null, null, 1, 1, 1, null, null, null, 2, null, null, null, 3, 3, 3, null, 4, 4, null, 5, 5, 5, 5, null, 6, 6, 6, 6, null, 7, 7, 7, null, 8, 8, 8, 8, 9, 9]);
            Assert.Equal(
                [0, 0, 9, 9, 8, 1, 1, 1, 8, 8, 8, 2, 7, 7, 7, 3, 3, 3, 6, 4, 4, 6, 5, 5, 5, 5, 6, 6],
                result
            );
        }

        private static long?[] ToDataArray(IEnumerable<BlockPair> blockPair)
        {
            return blockPair.SelectMany((p, i) => Enumerable.Repeat<long?>(i, (int)p.Data).Concat(Enumerable.Repeat<long?>(null, (int)p.Free))).ToArray();
        }

        [Fact]
        public void DataArrayTest()
        {
            Assert.Equal(
                [0, 0, null, null, null, 1, 1, 1, null, null, null, 2, null, null, null, 3, 3, 3, null, 4, 4, null, 5, 5, 5, 5, null, 6, 6, 6, 6, null, 7, 7, 7, null, 8, 8, 8, 8, 9, 9],
                ToDataArray([new(2, 3), new(3, 3), new(1, 3), new(3, 1), new(2, 1), new(4, 1), new(4, 1), new(3, 1), new(4, 0), new(2, 0)]));
        }


        [Theory]
        [InlineData(["../../../Day09/Example.txt", 1928])]
        [InlineData(["../../../Day09/Challenge.txt", 6288707484810])]
        public void Part1(string fileName, long expected)
        {
            Assert.Equal(expected, CalculateCheckSum(DefragArray(ToDataArray(GetInput(fileName)))));
        }

        [Theory]
        [InlineData(["../../../Day09/Example.txt", 2858])]
        [InlineData(["../../../Day09/Challenge.txt", 6311837662089])]
        public void Part2(string fileName, long expected)
        {
            Assert.Equal(expected, CalculateCheckSum(DefragArrayWholeFiles(ToDataArray(GetInput(fileName)))));
        }
    }
}