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

    private static Input Move(Input state, Point move, HashSet<Point> wallPoints)
    {
        var boxesState = state.Boxes.ToList();
        var boxLookup = boxesState
            .SelectMany(box => box
                .AsSinglePoints()
                .Select(point => (point, box)))
            .ToLookup(pair => pair.point, pair => pair.box);
        var affectedBoxes = Enumerable.Range(0, int.MaxValue)
            .AggregateList(new List<WidePoint> { state.Robot }, (agg, _) =>
            {
                var current = agg.SelectMany(point => point.AsSinglePoints()).ToHashSet();
                return agg
                    .Select(point => point + move)
                    .SelectMany(point => point.AsSinglePoints())
                    .Where(point => !current.Contains(point))
                    .SelectMany(point => boxLookup[point])
                    .ToList();
            })
            .Skip(1)
            .TakeWhile(boxes => boxes.Count > 0)
            .SelectMany(x => x)
            .ToList();
        affectedBoxes = affectedBoxes.Distinct().ToList();
        var unaffectedBoxes = boxesState
            .Where(box => !affectedBoxes.WideContains(box)).ToList();
        var newBoxes = unaffectedBoxes.Concat(affectedBoxes.Select(box => box + move)).ToArray();

        var newRobot = state.Robot + move;
        var result = new Input[] {
                state with { Robot = newRobot, Boxes = newBoxes },
                state }
            .ElementAt(Convert.ToInt32(newBoxes
                .Append(newRobot)
                .SelectMany(p => p
                    .AsSinglePoints())
                .ToHashSet()
                .Intersect(wallPoints).Any()));
        return result;
    }

    [Theory]
    [InlineData(["../../../Day15/Example1.txt", 2028])]
    [InlineData(["../../../Day15/Example2.txt", 10092])]
    [InlineData(["../../../Day15/Challenge.txt", 1471826])]
    public void Part1(string file, long expected)
    {
        var input = GetInput(file);
        var wallPoints = input.Walls.SelectMany(w => w.AsSinglePoints()).ToHashSet();
        var result = input.Moves.Aggregate(input with { Moves = [], Walls = [] }, (agg, i) => Move(agg, i, wallPoints));
        Assert.Equal(expected, CalculatSumOfAllBoxes(result.Boxes));
    }

    public static WidePoint Translate(WidePoint point)
    {
        return point with { Xs = [point.Xs[0] * 2] };
    }

    public static WidePoint[] Widen(WidePoint[] points)
    {
        return points.Select(Translate).Select(point => point with { Xs = [point.Xs[0], point.Xs[0] + 1] }).ToArray();
    }

    public static Input Widen(Input input)
    {
        return input with
        {
            Robot = Translate(input.Robot),
            Boxes = Widen(input.Boxes),
            Walls = Widen(input.Walls)
        };
    }

    [Theory]
    [InlineData(["../../../Day15/Example2.txt", 9021])]
    [InlineData(["../../../Day15/Challenge.txt", 1457703])]
    public void Part2(string file, long expected)
    {
        var input = Widen(GetInput(file));
        var wallPoints = input.Walls.SelectMany(w => w.AsSinglePoints()).ToHashSet();
        var result = input.Moves.Aggregate(input with { Moves = [] }, (agg, i) => Move(agg, i, wallPoints));
        Assert.Equal(expected, CalculatSumOfAllBoxes(result.Boxes));
    }
}