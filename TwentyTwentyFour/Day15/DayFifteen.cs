using TwentyTwentyFour.Day06;
using TwentyTwentyFour.Utils;

namespace TwentyTwentyFour.Day15;

public record Input(WidePoint Robot, WidePoint[] Boxes, WidePoint[] Walls, Point[] Moves);



public class DayFifteen
{
    private static Input GetInput(string file)
    {
        var split = File.ReadAllText(file)
            .Split("\n\n").ToArray();
        var mapDict = split[0].Split('\n')
            .SelectMany((row, y) => row
                .Select((c, x) => new { Character = c, Position = new WidePoint([x], y) }))
            .ToLookup(cell => cell.Character, cell => cell.Position);

        var moveDict = new Dictionary<char, Point> {
            {'^', new Point(0, -1) },
            {'v', new Point(0, 1) },
            {'<', new Point(-1, 0) },
            {'>', new Point(1, 0) },
        };
        return new Input(mapDict['@'].Single(), mapDict['O'].ToArray(), mapDict['#'].ToArray(), split[1].Split('\n').SelectMany(line => line.Select(c => moveDict[c])).ToArray());
    }

    private static long CalculatSumOfAllBoxes(IEnumerable<WidePoint> boxes)
    {
        return boxes.Sum(box => box.Y * 100 + box.Xs.Min());
    }

    private static Input Move(Input state, Point move)
    {
        var newRobot = state.Robot + move;
        var boxesState = state.Boxes.ToHashSet();
        var affectedTiles = Enumerable.Range(0, int.MaxValue)
            .Select(i => newRobot + move * i)
            .TakeWhileIncluding(boxesState.WideContains).ToList();
        var affectedBoxes = affectedTiles.SkipLast(1).ToList();
        var newBoxes = boxesState.WideExcept(affectedBoxes).Concat(affectedBoxes.Select(box => box + move)).ToArray();

        return new Input[] { state with { Robot = newRobot, Boxes = newBoxes }, state }.ElementAt(affectedTiles.TakeLast(1).Count(state.Walls.WideContains));
    }

    [Theory]
    [InlineData(["../../../Day15/Example1.txt", 2028])]
    [InlineData(["../../../Day15/Example2.txt", 10092])]
    [InlineData(["../../../Day15/Challenge.txt", 1471826])]
    public void Part1(string file, long expected)
    {
        var input = GetInput(file);
        Assert.Equal(expected, CalculatSumOfAllBoxes(input.Moves.Aggregate(input, Move).Boxes));
    }
}