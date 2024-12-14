using System.Text.RegularExpressions;

namespace TwentyTwentyFour;

public class DayThree
{
    static public string GetText()
    {
        return File.ReadAllText("../../../DayThreeInput.txt");
    }

    [Fact]
    void Part1()
    {
        Assert.Equal(165225049, GetMultipliedSum());
    }

    private static long GetMultipliedSum()
    {
        return Regex.Matches(GetText(), @"mul\((?<first>\d{1,3}),(?<second>\d{1,3})\)")
                    .Where(match => match.Success)
                    .Select(match => long.Parse(match.Groups["first"].Value) * long.Parse(match.Groups["second"].Value))
                    .Sum();
    }
}
