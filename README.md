Notes and examples of a workshop on LINQ I did for my co-workers.

Install [LINQPad](https://www.linqpad.net/) in order to run the example files.

```
PS> choco install linqpad5
```

Checkout the branch `about-to-fall-in-love` to implement the LINQ solutions yourself.

```
PS> git checkout about-to-fall-in-love
```

### 0. Why I hate loops

Can you see what the `for` loop below does? No? Me neither. This just goes to show what ugly kind of things are possible with a `for`/`foreach` loop.

```C#
var a = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

var x = 0;

for (var i = 0; i < a.Length; i++) {
    if (i % 3 == 0 && i > 0)
        a[i - 1] += a[i];
    
    if (a[i] % 2 == 0) {
        --x;
        continue;
    } 
    
    a[i] += x;
}

a.Dump();
```

Of course, it's also possible to write clean code using `for` loops, as well it is possible to write dirty code using LINQ. However, one of the two promotes abstraction, modularization, and immutability more than the other. Guess which one I'm talking about.

### 1. The only acceptable `for` loop

The only acceptable `for` loop is the one that has only one statement in it's body and does not need curly braces. In fact it should be enforced that loops are defined without curly braces to make sure they always have single-statement bodies.

```C#
void Main() {
    var foos = new[] { new Foo(), null, new Foo() };
    EnsureAllInitialized(foos);
    foos.Dump();
}

void EnsureAllInitialized(Foo[] foos) {
    for (var i = 0; i < foos.Length; i++)
        foos[i] = foos[i] ?? new Foo();
}

class Foo { }
```

Make the loop body a method in case it needs more statements. All dependencies should be parameters of that method.

Still a for loop should only be used when the performance gain due to in-place editing of collections really is needed.

### 2. The only acceptable `foreach` loop

The only acceptable `foreach` loop is the one being used to implement an extension method on `IEnumerable<T>` that takes an action with side-effects and executes it against each element. 

This extension method can be appended to the end of your LINQ operator chain as fluently as any other LINQ operator.

```C#
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
```
