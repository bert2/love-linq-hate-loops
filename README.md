### -1. Introduction

LINQ has been around in C# for quite some time now. Still many programmers prefer explicit looping with `for` and `foreach` in situations that also could have been solved using LINQ operators.

I claim that to every problem that seemingly requires loops there is a LINQ solution which is cleaner. As a matter of fact, I believe that loops should be avoided at all costs and only be used in very rare special cases.

The following examples show LINQ solutions to common looping problems while pointing out the issues of loops.

Install [LINQPad](https://www.linqpad.net/) in order to run the example files.

```
PS> choco install linqpad5
```

Checkout the branch `about-to-fall-in-love` to implement the LINQ solutions yourself.

```
PS> git checkout about-to-fall-in-love
```

### 0. Why I hate loops

Can you see what the `for` loop below does? No? Me neither. This just goes to show what ugly kind of things are possible with loops.

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

Have you ever considered the level of abstraction loops offer? It's actually astonishingly low. In essence they are just syntactic sugar for a jump and a branch instruction which both wrap a couple more instructions. Sure, it's efficient. But so is assembly language.

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

### 3. Map

`Select()` is your bread...

```C#
void Main() {
    var input = new[] { "1", "2", "3" };
    MapWithLoop(input).Dump();
    MapWithLinq(input).Dump();
}

List<int> MapWithLoop(string[] input) {
    var result = new List<int>();
    
    foreach (var item in input)
        result.Add(int.Parse(item));
    
    return result;
}

IEnumerable<int> MapWithLinq(string[] input) => input
    .Select(int.Parse);
```

### 4. Filter

...and `Where()` is your butter.

```C#
void Main() {
    var input = new[] { 1, 2, 3, 4 };
    FilterWithLoop(input).Dump();
    FilterWithLinq(input).Dump();
}

List<int> FilterWithLoop(int[] input) {
    var result = new List<int>();

    foreach (var item in input)
        if (item % 2 == 0)
            result.Add(item);

    return result;
}

IEnumerable<int> FilterWithLinq(int[] input) => input
    .Where(x => x % 2 == 0);
```

### 5. Capture index

Sometimes you need the element index in your mapping function. Sounds like the perfect job for a classic `for` loop, you think? Not on my watch!

LINQ operators usually have overloads that feed the element along with its index to your `Func` if you need them.

The example below converts a binary number to decimal. Using the index of the bits this is easy enough, but we have to keep in mind that the least significant bit is the _last_ element of the input. Of course we can work out the correct process with a `for` loop (after fixing this damn off-by-one error...), but see how natural you can express this reversal with LINQ.

```C#
void Main() {
    var input = new[] { 1, 1, 0, 1 };
    CaptureIndexWithLoop(input).Dump();
    CaptureIndexWithLinq(input).Dump();
}

int CaptureIndexWithLoop(int[] input) {
    var result = 0;
    var lastIndex = input.Length - 1;
    
    for (var i = 0; i < input.Length; i++)
        result += MulByNthPowerOfTwo(input[i], lastIndex - i);

    return result;
}

int CaptureIndexWithLinq(int[] input) => input
    .Reverse()
    .Select(MulByNthPowerOfTwo)
    .Sum();

int MulByNthPowerOfTwo(int x, int n) => x << n;
```

### 6. Flatten

Flattening a list of lists is a fairly common operation. In loop land this is done with nesting. In LINQtopia however, it's just a call to `SelectMany()`.

```C#
void Main() {
    var input = new[] { 
        new[] { 1, 2 }, 
        new[] { 10, 20 },
        new[] { 100, 200 }
    };

    FlattenWithLoop(input).Dump();
    FlattenWithLinq(input).Dump();
}

List<int> FlattenWithLoop(int[][] input) {
    var result = new List<int>();

    foreach (var items in input)
        foreach (var item in items)
            result.Add(item);

    return result;
}

IEnumerable<int> FlattenWithLinq(int[][] input) => input
    .SelectMany(items => items);
```

Note how this scales when applied to lists of lists of lists of... With loops you will need even more _nesting_, whereas with LINQ you keep _chaining_ `SelectMany()` until you reach the desired depth.

### 7. Carry index

The previous examples were all fine and dandy, but this is were programmers used to looping begin to object.

First, make sure you understand how the indices of the `TestResult`s are paired with each element of the inner collections by running the example.

> ```
> +---------------------------+
> |Place #2 used amphetamines.|
> +---------------------------+
> |Place #2 used steroids.    |
> +---------------------------+
> |Place #3 used steroids.    |
> +---------------------------+
>
> ...
> ```

```C#
void Main() {
    var input = new[] {
        new TestResult { Runner = "Road Runner", Drugs = new string[] { } },
        new TestResult { Runner = "Flash", Drugs = new[] { "amphetamines", "steroids" } },
        new TestResult { Runner = "Sonic", Drugs = new[] { "steroids" } }
    };

    CarryIndexWithLoop(input).Dump();
    CarryIndexWithLinq(input).Dump();
    CarryIndexWithDirtyLinq(input).Dump();
}

List<string> CarryIndexWithLoop(TestResult[] input) {
    var result = new List<string>();

    for (var place = 0; place < input.Length; place++)
        foreach (var drug in input[place].Drugs)
            result.Add(MakeReportLine(place, drug));

    return result;
}

IEnumerable<string> CarryIndexWithLinq(TestResult[] input) => input
    .Select((testResult, place) => (testResult, place))
    .SelectMany(
        x => x.testResult.Drugs, 
        (x, drug) => MakeReportLine(x.place, drug));

IEnumerable<string> CarryIndexWithDirtyLinq(TestResult[] input) => input
    .SelectMany((testResult, place) => testResult
        .Drugs.Select(drug => MakeReportLine(place, drug)));

class TestResult {
    public string Runner { get; set; }
    public string[] Drugs { get; set; }
}

string MakeReportLine(int place, string drug) => $"Place #{place + 1} used {drug}.";
```

Carrying the index from the outer loop into the inner loop's body is straightforward and doesn't look much different than any other nested loop. In LINQ however, this may not come as easy at first, because you explicitely have to "push" the index into the next operator.

But what seems like a nuisance actually makes your life easier. Implicitely being able to use variables from outer scopes also increases the amount of information readers of your code have to keep track of. By explicitely declaring what goes into the next operator, you are not only announcing what the important parts of the input data are. You are also keeping the scope of the following operator clean and tidy, making it easier to reason about.

Don't try and boost your operator's scope by nesting lambdas. This is basically the same way the looping solution works and we want to do better than that.

### Cross join

When you see nested loops working away on two collections, chances are they are both being joined somehow. The question is: what kind of join are you looking at?

When done with loops it's not obvious. Often a develooper (sorry) will "invent" his or her own logic that looks like someone was trying to build the death star. This kind of DIY joining does nothing but obfuscate intent and makes your reviewer question your mental stability.

Always figure out what kind of join your code should perform _before_ you start coding. Then express this operation in abstract terms (cross, inner, left, right, full). Those are concepts that your readers can google if they don't know them.

The simplest kind of join is the cross join (a.k.a. cartesian product): everything is paired with everthing. In LINQ it's just a call to our old friend `SelectMany()`.

```C#
void Main() {
    var input1 = new[] { 'A', 'B', 'C' };
    var input2 = new[] { 1, 2, 3 };
    CrossJoinWithLoop(input1, input2).Dump();
    CrossJoinWithLinq(input1, input2).Dump();
}

List<string> CrossJoinWithLoop(char[] input1, int[] input2) {
    var result = new List<string>();

    foreach (var item1 in input1)
        foreach (var item2 in input2)
            result.Add($"{item1}{item2}");
    
    return result;
}

IEnumerable<string> CrossJoinWithLinq(char[] input1, int[] input2) => input1
    .SelectMany(_ => input2, (x1, x2) => $"{x1}{x2}");
```

### Inner join

The inner join is also a fairly simple join. It's basically the intersection of two lists based on a common key.

The inner join is so prevalent that LINQ has it's own operator for it: `Join()`. There is no excuse not to use it. Especially considering that the inner join is often implemented rather ineffeciently when done by looplanders. Can you spot the performance issue in the loop example?

The nested loops look innocent, but we do a linear search there in order to find the matching item in `input2`. Madness! That's `O(NM)`! Converting `input2` to a `Lookup<Bar>` beforehand will give us `O(N)`.

Or we can just use `Join()` and let LINQ do the busywork for us...

```C#
void Main() {
    var input1 = new List<Foo> { 
        Foo.New(1, "A"), Foo.New(2, "B"), Foo.New(3, "C") 
    };
    var input2 = new List<Bar> { 
        Bar.New(1, 100), Bar.New(2, 200), Bar.New(2, 201), Bar.New(4, 300) 
    };
    InnerJoinWithLoop(input1, input2).Dump();
    InnerJoinWithLinq(input1, input2).Dump();
}

List<string> InnerJoinWithLoop(List<Foo> input1, List<Bar> input2) {
    var result = new List<string>();

    foreach (var item1 in input1)
        foreach (var item2 in input2.FindAll(x => x.Id == item1.Id))
            result.Add($"{item1.Value}{item2.Value}");

    return result;
}

IEnumerable<string> InnerJoinWithLinq(List<Foo> input1, List<Bar> input2) => input1
    .Join(
        input2, 
        f => f.Id, 
        b => b.Id, 
        (f, b) => $"{f.Value}{b.Value}");

class Foo {
    public static Foo New(int id, string v) => new Foo { Id = id, Value = v };
    public int Id { get; set; }
    public string Value { get; set; }
}

class Bar {
    public static Bar New(int id, int v) => new Bar { Id = id, Value = v };
    public int Id { get; set; }
    public int Value { get; set; }
}
```
