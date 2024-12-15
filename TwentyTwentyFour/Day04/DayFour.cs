using System.Text.RegularExpressions;
using Xunit.Sdk;

namespace TwentyTwentyFour;

public class DayFour
{
    private static char[][] GetGrid()
    {
        return File.ReadAllLines("../../../Day04/DayFourInput.txt")
            .Select(line => line.ToArray())
            .ToArray();
    }

    private static char[][] Rotate(char[][] grid)
    {
        return Enumerable.Range(0, grid[0].Length).Select(x => Enumerable.Range(0, grid.Length).Select(y => grid[y][x]).Reverse().ToArray()).ToArray();
    }

    private static IEnumerable<string> GetOneDirectionalSearchableLines(char[][] grid, bool includeNonDiagonal)
    {
        return grid.Select(row => new string(row)).Take(grid.Length * Convert.ToInt32(includeNonDiagonal)).Concat(
            grid.Select((row, y) => new string(row.Select((_, x) => grid[y + x][x]).Take(grid.Length - y).ToArray()))).Concat(
            grid[0].Select((_, x) => new string(grid.Select((_, y) => grid[y][y + x]).Take(grid[0].Length - x).ToArray())).Skip(1));
    }

    private static IEnumerable<string> GetCrossSearchableLines(char[][] grid, bool includeNonDiagonal)
    {
        return GetOneDirectionalSearchableLines(grid, includeNonDiagonal).Concat(GetOneDirectionalSearchableLines(Rotate(grid), includeNonDiagonal));
    }

    private static IEnumerable<string> GetBackAndForwardLines(char[][] grid, bool includeNonDiagonal)
    {
        var oneDirection = GetCrossSearchableLines(grid, includeNonDiagonal);
        return oneDirection.Concat(oneDirection.Select(line => new string(line.Reverse().ToArray())));
    }

    private static long CountXmas(char[][] grid)
    {
        return GetBackAndForwardLines(grid, true).SelectMany(line => Regex.Matches(line, "XMAS")).Select(match => match.Success.CompareTo(false)).Sum();
    }

    private static long CountXMas(char[][] grid)
    {
        return grid.SelectMany((row, y) => row.Select((_, x) => (x, y)))
            .Select(point => grid.Skip(point.y).Take(3).Select(row => row.Skip(point.x).Take(3).ToArray()).ToArray())
            .Select(tgrid => GetBackAndForwardLines(tgrid, false)
                .Where(line => line.Equals("MAS")))
            .Select(lines => lines.Count())
            .Select(count => count.CompareTo(2))
            .Sum(comp => 1 - Math.Abs(comp));
    }

    [Fact]
    public void Part1()
    {
        Assert.Equal(2390, CountXmas(GetGrid()));
    }

    [Fact]
    public void Part2()
    {
        Assert.Equal(1809, CountXMas(GetGrid()));
    }



}