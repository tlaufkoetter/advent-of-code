namespace TwentyTwentyFour.Day07;

public class DaySeven
{

    static long Add(long a, long b)
    {
        return a + b;
    }

    static long Mult(long a, long b)
    {
        return a * b;
    }

    static long Concat(long a, long b)
    {
        return long.Parse($"{a}{b}");
    }

    record Equation(long Result, long[] Coefficients);


    public long BranchlessFoldLeft(Func<long, long, long>[] operands)
    {
        var lines = File.ReadAllLines("../../../Day07/DaySevenInput.txt");
        var equations = lines
            .Select(ParseEquation)
            .Where(HasSolution);
        return AggregateSolutions(equations);

        Equation ParseEquation(string line)
        {
            var parts = line.Split(':')
                    .SelectMany(part => part.Trim().Split(' '))
                    .Where(part => !string.IsNullOrWhiteSpace(part))
                    .Select(long.Parse);
            return new Equation(parts.First(), parts.Skip(1).ToArray());
        }

        long AggregateSolutions(IEnumerable<Equation> equations)
        {
            return equations.Where(HasSolution).Sum(equation => equation.Result);
        }

        Equation Advance(Equation equation, Func<long, long, long> operand)
        {
            return equation with { Result = operand(equation.Result, equation.Coefficients.First()), Coefficients = equation.Coefficients.Skip(1).ToArray() };
        }


        IEnumerable<long> CalculateSolutions(Equation equation)
        {
            var firsts = equation.Coefficients.Take(1).ToArray();
            return new long[] { equation.Result }.Take(Math.Abs(firsts.Length - 1))
                .Concat(firsts.SelectMany(_ => operands.SelectMany(operand => CalculateSolutions(Advance(equation, operand)))));
        }

        bool HasSolution(Equation equation)
        {
            return CalculateSolutions(Advance(equation with { Result = 0 }, Add)).Contains(equation.Result);
        }
    }

    [Fact]
    public void Part1()
    {
        Assert.Equal(663613490587, BranchlessFoldLeft([Add, Mult]));
    }

    [Fact]
    public void Part2()
    {
        Assert.Equal(110365987435001, BranchlessFoldLeft([Add, Mult, Concat]));
    }
}