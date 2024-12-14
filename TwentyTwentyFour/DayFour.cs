using System.Text.RegularExpressions;
using Xunit.Sdk;

namespace TwentyTwentyFour;

public class DayFour
{
    private static char[][] GetGrid()
    {
        return File.ReadAllLines("../../../DayFourInput.txt")
            .Select(line => line.ToArray())
            .ToArray();
    }

    private static char[][] Rotate(char[][] grid)
    {
        return Enumerable.Range(0, grid[0].Length).Select(x => Enumerable.Range(0, grid.Length).Select(y => grid[y][x]).Reverse().ToArray()).ToArray();
    }

    private static IEnumerable<string> GetOneDirectionalSearchableLines(char[][] grid)
    {
        return grid.Select(row => new string(row)).Concat(
            grid.Select((row, y) => new string(row.Select((_, x) => grid[y + x][x]).Take(grid.Length - y).ToArray()))).Concat(
            grid[0].Select((_, x) => new string(grid.Select((_, y) => grid[y][y + x]).Take(grid[0].Length - x).ToArray())).Skip(1));
    }

    private static IEnumerable<string> GetCrossSearchableLines(char[][] grid)
    {
        return GetOneDirectionalSearchableLines(grid).Concat(GetOneDirectionalSearchableLines(Rotate(grid)));
    }

    private static IEnumerable<string> GetBackAndForwardLines(char[][] grid)
    {
        var oneDirection = GetCrossSearchableLines(grid);
        return oneDirection.Concat(oneDirection.Select(line => new string(line.Reverse().ToArray())));
    }

    private static long CountXmas(char[][] grid)
    {
        return GetBackAndForwardLines(grid).SelectMany(line => Regex.Matches(line, "XMAS")).Select(match => match.Success.CompareTo(false)).Sum();
    }

    [Fact]
    public void Part1()
    {
        Assert.Equal(2390, CountXmas(GetGrid()));
    }



}