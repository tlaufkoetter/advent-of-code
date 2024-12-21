using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        [Fact]
        public void TestCheckSum()
        {
            Assert.Equal(1928, CalculateCheckSum([0, 0, 9, 9, 8, 1, 1, 1, 8, 8, 8, 2, 7, 7, 7, 3, 3, 3, 6, 4, 4, 6, 5, 5, 5, 5, 6, 6]));
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
    }
}