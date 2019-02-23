<Query Kind="Program" />

/* The only acceptable `foreach` loop is the one being used to implement an 
   extension method on `IEnumerable<T>` that takes an action with side-effects 
   and executes it against each element. 
   
   This extension method can be appended to the end of your LINQ operator chain as
   fluently as any other LINQ operator.
   
   Do not use `ForEach()` as a mere replacement for `foreach`! Using `ForEach()` 
   implies that you are mutating state with side-effects, which should happen 
   sparingly. Remember: side-effects are dirty; it's immutability we are after. */

void Main() {
    var tokens = new[] { "Hello", ",", " ", "world", "!" };
    tokens.ForEach(Console.Write);
}

public static class EnumerableExtensions {
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
        foreach (var item in source)
            action(item);
    }
}