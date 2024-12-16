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

        public static IEnumerable<TResult> AggregateList<TResult, TSource>(this IEnumerable<TSource> source, TResult seed, Func<TResult, TSource, TResult> transform)
        {
            var current = seed;
            return source.Select(s =>
            {
                current = transform(current, s);
                return current;
            })
            .Prepend(seed);
        }

        public static IEnumerable<TSource> TakeWhileIncluding<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var enumerator = source.GetEnumerator();
            var areElementsLeft = false;
            var whytho = Enumerable.Repeat(false, int.MaxValue)
                .TakeWhile(_ =>
                {
                    areElementsLeft = enumerator.MoveNext();
                    return areElementsLeft;
                })
                .Select(_ => enumerator.Current)
                .TakeWhile(predicate);

            return whytho.Concat(Enumerable.Repeat(false, 1)
                .TakeWhile(_ => areElementsLeft)
                .Select(_ => enumerator.Current));
        }
    }
}