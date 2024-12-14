
Console.WriteLine(File.ReadAllLines("input.txt")
    .Select(line => line.Split(':')
        .SelectMany(part => part.Trim().Split(' '))
        .Where(part => !string.IsNullOrWhiteSpace(part))
        .Select(long.Parse))
    .Select(parts => new Equation(parts.First(), parts.Skip(1).ToArray()))
    .Where(HasSolution)
    .Sum(equation => equation.Result));


bool HasSolution(Equation equation)
{
    return equation.Result == 0 && equation.Coefficients.Length == 0
    || equation.Coefficients.Length != 0 && equation.Result > 0
        && (HasSolution(equation with { Result = equation.Result - equation.Coefficients.Last(), Coefficients = equation.Coefficients.SkipLast(1).ToArray() })
            || equation.Result % equation.Coefficients.Last() == 0 && HasSolution(equation with { Result = equation.Result / equation.Coefficients.Last(), Coefficients = equation.Coefficients.SkipLast(1).ToArray() }));
}
record Equation(long Result, long[] Coefficients);


