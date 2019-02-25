<Query Kind="Program" />

/* What throws a lot of people off when starting with LINQ is it's laziness. Take 
   the LINQ chain below for example. It increments each number in the array by 
   one, squares them, and then prints them to the console using `Tap()`. But when
   you run the example in LINQPad nothing happens. There is no output whatsoever.
   Why is that?
   
   The answer is that LINQ operators only begin their work when something 
   actually requests a value from them. This is called _deferred execution_: you 
   can chain as many LINQ operators as you like and even pass that query around 
   in a variable, but as long as no one does "something" with its values, nothing 
   will be executed.
   
   That "something" that has to be done is _enumeration_ and is usually 
   initiated by a `foreach` loop iterating over the `IEnumerable` returned by
   a LINQ chain. Of course it doesn't have to be an explicit `for`/`foreach` loop.
   Some LINQ operators, like `ToList()` and our `ForEach()`, do the looping 
   implicitly.
   
   Enumerating the values of a LINQ chain triggers a process called 
   _materialization_ where the last operator of the chain is queried for a value 
   by the enumerating loop. This operator will in turn request a value from its 
   input, which might be another operator preceeding it. The chain is traversed 
   backwards up until an `IEnumerable` is reached that acts as the 
   _value generator_ of the chain. Such a generator could be anything from an 
   in-memory collection, a specialized generator function like 
   `Enumerable.Range()`, the filesystem, a database query; you name it.
   
   As soon as the first value was generated it will be fed to the first operator 
   in the chain, which will do its work and then forward it to the next operator.
   This continues until the value reaches the _enumerator_. When the enumerator
   requests the next value the same process will start over again.
   
   If you check the implementation of `Tap()` now, you will notice that it does 
   not do any looping at all. It just adds another `Select()` to the chain. 
   That's why there is nothing happening. We cannot use `Tap()` to terminate our 
   chain, because it cannot act as an enumerator. We have to use `ForEach()`.
   
   The rule of thumb is that an operator returning `IEnumerable<T>` will deferr 
   execution and an operator returning `void` will enumerate. */

void Main() {
    new[] { 1, 2, 3 }
        .Select(x => x + 1)
        .Select(x => x * x)
        .Tap(Console.WriteLine);
}

public static class Extensions {
    public static IEnumerable<T> Tap<T>(
        this IEnumerable<T> source, Action<T> action)
        => source.Select(x => { action(x); return x; });
}