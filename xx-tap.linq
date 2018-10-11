<Query Kind="Program" />

void Main() {
    new[] {1, 2, 3}
        .Tap(Console.Write)
        .Dump();
}

public static class EnumerableExtensions {
    public static IEnumerable<T> Tap<T>(this IEnumerable<T> source, Action<T> action) =>
        source.Select(Execute(action));

    private static Func<T, T> Execute<T>(Action<T> action) =>
        x => {
            action(x);
            return x;
        };
}
