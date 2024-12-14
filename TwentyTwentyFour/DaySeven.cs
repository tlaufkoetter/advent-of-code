namespace TwentyTwentyFour;

public class DaySeven
{
    record Equation(long Result, long[] Coefficients);
    [Fact]
    public void BranchlessFoldLeft()
    {
        var lines = File.ReadAllLines("../../../DaySevenInput.txt");
        var equations = lines
            .Select(ParseEquation)
            .Where(HasSolution);
        var solution = AggregateSolutions(equations);
        Assert.Equal(663613490587, solution);

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

        long Add(long a, long b)
        {
            return a + b;
        }

        long Mult(long a, long b)
        {
            return a * b;
        }


        IEnumerable<long> CalculateSolutions(Equation equation)
        {
            Func<long, long, long>[] operands = [Add, Mult];
            var firsts = equation.Coefficients.Take(1).ToArray();
            return new long[] { equation.Result }.Take(Math.Abs(firsts.Length - 1))
                .Concat(firsts.SelectMany(_ => operands.SelectMany(operand => CalculateSolutions(Advance(equation, operand)))));
        }

        bool HasSolution(Equation equation)
        {
            return CalculateSolutions(Advance(equation with { Result = 0 }, Add)).Contains(equation.Result);
        }
    }
}