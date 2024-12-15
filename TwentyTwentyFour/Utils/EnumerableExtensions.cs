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

        public static IEnumerable<Point> AsPoints(this WidePoint point)
        {
            return point.Xs.Select(x => new Point(x, point.Y));
        }
        public static bool WideContains(this HashSet<WidePoint> source, WidePoint value)
        {
            var points = source.SelectMany(point => point.AsPoints()).ToHashSet();
            return value.AsPoints().Any(points.Contains);
        }

        public static bool WideContains(this IEnumerable<WidePoint> source, WidePoint value)
        {
            return source.ToHashSet().WideContains(value);
        }

        public static IEnumerable<WidePoint> WideExcept(this IEnumerable<WidePoint> source, IEnumerable<WidePoint> values)
        {
            var points = values.SelectMany(point => point.AsPoints()).ToHashSet();
            return source.Where(point => point.AsPoints().All(p => !points.Contains(p)));
        }
    }
}