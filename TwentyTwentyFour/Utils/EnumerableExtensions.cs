namespace TwentyTwentyFour.Utils
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TResult> AsRange<TResult>(this Func<TResult, TResult> transform, TResult seed, int count)
        {
            var current = seed;
            return Enumerable.Repeat(false, count).Select(_ =>
            {
                current = transform(current);
                return current;
            })
            .Prepend(seed);
        }

    }
}