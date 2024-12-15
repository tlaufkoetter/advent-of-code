using TwentyTwentyFour.Day06;
using TwentyTwentyFour.Utils;

namespace TwentyTwentyFour.Day15;

public record Input(Point Robot, Point[] Boxes, Point[] Walls, Point[] Moves);



public class DayFifteen
{
    private static Input GetInput(string file)
    {
        var split = File.ReadAllText(file)
            .Split("\n\n").ToArray();
        var mapDict = split[0].Split('\n')
            .SelectMany((row, y) => row
                .Select((c, x) => new { Character = c, Position = new Point(x, y) }))
            .ToLookup(cell => cell.Character, cell => cell.Position);

        var moveDict = new Dictionary<char, Point> {
            {'^', new Point(0, -1) },
            {'v', new Point(0, 1) },
            {'<', new Point(-1, 0) },
            {'>', new Point(1, 0) },
        };
        return new Input(mapDict['@'].Single(), mapDict['O'].ToArray(), mapDict['#'].ToArray(), split[1].Split('\n').SelectMany(line => line.Select(c => moveDict[c])).ToArray());
    }

    private static long CalculatSumOfAllBoxes(IEnumerable<Point> boxes)
    {
        return boxes.Sum(box => box.Y * 100 + box.X);
    }

    private static Input Move(Input state, Point move)
    {
        var newRobot = state.Robot + move;
        var boxesState = state.Boxes.ToHashSet();
        var affectedTiles = Enumerable.Range(0, int.MaxValue)
            .Select(i => newRobot + move * i)
            .TakeWhileIncluding(boxesState.Contains).ToList();
        var affectedBoxes = affectedTiles.SkipLast(1).ToList();
        var newBoxes = boxesState.Except(affectedBoxes).Concat(affectedBoxes.Select(box => box + move)).ToArray();

        return new Input[] { state with { Robot = newRobot, Boxes = newBoxes }, state }.ElementAt(affectedTiles.TakeLast(1).Count(state.Walls.Contains));
    }

    [Theory]
    [InlineData(["../../../Day15/Example1.txt", 2028])]
    public void Part1(string file, long expected)
    {
        var input = GetInput(file);
        Assert.Equal(expected, CalculatSumOfAllBoxes(input.Moves.Aggregate(input, Move).Boxes));
    }
}