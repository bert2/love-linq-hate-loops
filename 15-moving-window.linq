<Query Kind="Program" />

/* What if operating on list elements independently is not sufficient? What if 
   the element's neighbors need to be considered as well? 
   
   Truth is there is no nice way of doing this with LINQ. If you know you are
   dealing with `IList<T>`s then you can use the trick demonstrated below. But
   if your `IEnumerable<T>` does not have an indexer then `ElementAt()` will go
   through all the elements unitl it reaches the `i`th one and performance will 
   suffer.
   
   Alternatively you can use MoreLINQ's `Pairwise()`. I added it's implementation
   here so you can see how it looks when LINQ reaches its limits.
   
   If you have to resort to using the `IEnumerator<T>` of your `IEnumerable<T>`
   it's probably best that you generalize the code into a reusable extension 
   method and hide it somewhere. */

void Main() {
    var input = new[] { 1, 2, 3, 2, 1 };
    MovingWindowWithLoop(input).Dump();
    MovingWindowWithLinq1(input).Dump();
    MovingWindowWithLinq2(input).Dump();
}

List<int> MovingWindowWithLoop(int[] input) {
    var result = new List<int>();

    for (int i = 1; i < input.Length; i++) {
        result.Add(input[i] - input[i - 1]);
    }

    return result;
}

IEnumerable<int> MovingWindowWithLinq1(int[] input) => input
    .Skip(1)
    .Select((x, i) => x - input.ElementAt(i));

IEnumerable<int> MovingWindowWithLinq2(int[] input) => input
    .Pairwise((x1, x2) => x2 - x1);

public static class EnumerableExtensions {
    public static IEnumerable<TResult> Pairwise<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, TSource, TResult> resultSelector) {

        using (var e = source.GetEnumerator()) {
            if (!e.MoveNext())
                yield break;

            var previous = e.Current;
            while (e.MoveNext()) {
                yield return resultSelector(previous, e.Current);
                previous = e.Current;
            }
        }
    }
}
