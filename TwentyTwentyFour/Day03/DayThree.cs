using System.Text.RegularExpressions;

namespace TwentyTwentyFour.Day03;

public class DayThree
{
    static public string GetText()
    {
        return File.ReadAllText("../../../Day03/DayThreeInput.txt");
    }

    [Fact]
    void Part1()
    {
        Assert.Equal(165225049, GetMultipliedSum());
    }

    [Fact]
    void Part2()
    {
        Assert.Equal(108830766, GetEnabledMultipliedSum());
    }

    private static Dictionary<int, long> GetMuls(string text)
    {
        return Regex.Matches(GetText(), @"mul\((?<first>\d{1,3}),(?<second>\d{1,3})\)")
                    .Where(match => match.Success)
                    .ToDictionary(match => match.Index, match => long.Parse(match.Groups["first"].Value) * long.Parse(match.Groups["second"].Value));
    }

    private static Dictionary<int, bool> GetDos(string text)
    {
        return Regex.Matches(GetText(), @"do(n't)?\(\)")
                    .Where(match => match.Success)
                    .ToDictionary(match => match.Index, match => match.Value.Equals("do()"));
    }

    private static long GetMultipliedSum()
    {
        return GetMuls(GetText()).Values.Sum();
    }

    private static long GetEnabledMultipliedSum()
    {
        var text = GetText();
        var dos = GetDos(text);
        var muls = GetMuls(text);
        return dos.Keys.Concat(muls.Keys).Order().Aggregate((0L, true), (agg, i) =>
        {
            var doo = dos.Where(kvp => kvp.Key.Equals(i)).Select(kvp => kvp.Value).Append(agg.Item2).First();
            return (agg.Item1 + muls.Where(kvp => kvp.Key.Equals(i)).Sum(kvp => kvp.Value) * doo.CompareTo(false), doo);
        }).Item1;
    }
}
