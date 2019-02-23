<Query Kind="Program" />

/* The tap operator is helpful when you have to hook side effects lazily into 
   your operator chain.
   
   Think of it as tapping a phone line: you can listen in on all elements going
   down the chain without anyone below noticing. */

void Main() {
    new[] {1, 2, 3}
        .Select(x => x + 1)
        .Tap(Console.Write)
        .Select(x => x * x)
        .Dump();
}

public static class EnumerableExtensions {
    public static IEnumerable<T> Tap<T>(
        this IEnumerable<T> source, Action<T> action) 
        => source.Select(x => { action(x); return x; });
}