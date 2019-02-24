## Introduction

LINQ has been around in C# for quite some time now. Still many programmers prefer explicit looping with `for` and `foreach` in situations that also could have been solved using LINQ operators.

I claim that to every problem that seemingly requires loops there is a LINQ solution which is cleaner. As a matter of fact, I believe that loops should be avoided and only be used in rare special cases.

The following examples show LINQ solutions to common list transformation problems while pointing out the issues of using loops.

Install [LINQPad](https://www.linqpad.net/) in order to run the example files.

```
PS> choco install linqpad5
```

Checkout the branch `about-to-fall-in-love` to try and implement the LINQ solutions yourself.

```
PS> git checkout about-to-fall-in-love
```
### TL;DR

Jump straight to the [summary](#summary).

### Disclaimer

Some of my statements in this text might seem a bit absolute or even harsh. But after over 15 years of coding I've seen some pretty abhorrent nesting of loops in legacy code over and over again (including code I wrote myself) which leads to some frustration of course.

The sad part is though, that I've also seen the same nested loops happening in greenfield projects. Either because the developers are just used to this kind of code and don't know better, or because they feel inclined to take shortcuts due to pressure or laziness. And that's a big problem.

With this guide I'm hoping to do a small part in alleviating this problem, but take what I write here with a grain of salt.

### Table of contents

0. [Why I hate loops](#0)
1. [The only acceptable `for` loop](#1)
2. [The only acceptable `foreach` loop](#2)
3. [Map](#3)
4. [Filter](#4)
5. [Capture index](#5)
6. [Flatten](#6)
7. [Carry index](#7)
8. [Cross join](#8)
9. [Inner join](#9)
10. [Left join](#10)
11. [Right join](#11)
12. [Full join](#12)
13. [Conditional join](#13)
14. [Reduce](#14)
15. [Moving window](#15)
16. [Tap](#16)
17. [Modularization](#17)
18. [Laziness](#18)
19. [Summary](#summary)

<a name="0"></a>
## 0. Why I hate loops

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

Of course, it's also possible to write clean code using `for` loops, as well as it is possible to write dirty code using LINQ. However, LINQ promotes abstraction, modularization, and immutability more than loops do. LINQ does this by taking power away from you and in the beginning it might feel you are programming with your hands tied to your back. But in the long run you will end up with code that is easier to maintain.

Have you ever considered the level of abstraction loops offer? It's actually astonishingly low. In essence they are just syntactic sugar for an increment and a branch instruction that both wrap the instructions of your loop body. Sure, it's efficient. But so is coding in assembly language. Because loops are so low-level, some people even say that [the `for` loop will be the new `goto`]( https://www.quora.com/Can-we-say-that-the-for-loop-will-be-the-new-goto-%E2%80%9D-given-the-increasing-popularity-of-functional-programming/answer/Tikhon-Jelvis?share=1&srid=3SJB).

The lack of abstraction of loops forces readers to carefully track state in order to understand what your loop is doing. One cannot immediately tell whether the loop is for instance filtering or mapping the input list (or maybe it's doing both operations at once). With LINQ operators intent becomes more explicit, because a `Where()` will always filter and a `Select()` will always map.

The hard truth is though that most programmers, when given a problem that requires some kind of looping, will be fastest when coding it using `for`/`foreach` loops. I think that's because they already have an _abstract model_ of the problem in their heads and coding appears as the act of merely turning this model into _concrete instructions_. But that's where they are wrong. Because now it is up to the readers of their code to _reverse engineer_ that abstract model from a set of statements that really could do anything.

This is were LINQ helps. Using LINQ forces you to stay in the abstract realm more than loops do and delegates the dirty business with concrete instructions to the compiler.

In short: excessively relying on loops is like not writing tests. Sure, you will be faster now, but in the end your colleagues will hate you for it.

<a name="1"></a>
## 1. The only acceptable `for` loop

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

<a name="2"></a>
## 2. The only acceptable `foreach` loop

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

Do not use `ForEach()` as a mere replacement for `foreach`! Using `ForEach()` implies that you are mutating state with side-effects, which should happen sparingly. Remember: side-effects are dirty; it's immutability we are after.

<a name="3"></a>
## 3. Map

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

<a name="4"></a>
## 4. Filter

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

<a name="5"></a>
## 5. Capture index

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
    .Select((x, i) => MulByNthPowerOfTwo(x, i))
    .Sum();

int MulByNthPowerOfTwo(int x, int n) => x << n;
```

Notice how the parameters of the lambda in `Select((x, i) => ...`) match up with parameters of `MulByNthPowerOfTwo(int x, int n)`? This means we could leave them out entirely and write `Select(MulByNthPowerOfTwo)`, which is less redundant.

<a name="6"></a>
## 6. Flatten

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

<a name="7"></a>
## 7. Carry index

The previous examples were all fine and dandy, but here is were programmers used to looping begin to object.

The example pairs each element index (the place of a runner in a competition) with the elements of a nested collection (the drugs the runner used). We are _carrying_ the indeces down into a deeper level, so to speak. It's probably easier to understand if you just run the example right now and have a look at the output.

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
        new TestResult { Runner = "Road Runner", Drugs = new string[0] },
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

Also consider how both solutions scale if we for instance wanted to only list the drugs of the first and second places in our report: in the loop solution we'd introduce another level of nesting (`if (place > 3)`) inbetween the two loops; in the LINQ solution we'd add a filter (`Where(x => x.place > 3)`) to our chain without any increase in the nesting level.

Don't try and boost your operator's scope by nesting lambdas like `CarryIndexWithDirtyLinq()` does. This is basically the same way the looping solution works and we want to do better than that.

Of course, we are not restricted to carrying only indices. The same patterns works when you have to carry other data from the top level collection into the deeper levels of nested collections.

<a name="8"></a>
## 8. Cross join

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

<a name="9"></a>
## 9. Inner join

The inner join also is a fairly simple join. It's basically the intersection of two lists based on a common key.

The inner join is so prevalent that LINQ has it's own operator for it: `Join()`. There is no excuse not to use it. Especially considering that the inner join is often implemented rather ineffeciently when done by looplanders. Can you spot the performance issue in the loop example?

The nested loops look innocent, but we do a linear search there in order to find the matching item in `input2`. Madness! That's `O(NM)`! Converting `input2` to a `Lookup<Bar>` beforehand would give us `O(N)`.

Or we can just use `Join()` and let LINQ do the busywork for us...

```C#
void Main() {
    var input1 = new List<Foo> { 
        new Foo(1, "A"), new Foo(2, "B"), new Foo(3, "C") 
    };
    var input2 = new List<Bar> { 
        new Bar(1, 100), new Bar(2, 200), new Bar(2, 201), new Bar(4, 400) 
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
    public Foo(int id, string v) { Id = id; Value = v; }
    public int Id { get; set; }
    public string Value { get; set; }
}

class Bar {
    public Bar(int id, int v) { Id = id; Value = v; }
    public int Id { get; set; }
    public int Value { get; set; }
}
```

<a name="10"></a>
## 10. Left join

The left join is a bit more tricky to do with LINQ, because there is no inherent support for it.
   
Luckily we can use `GroupJoin()` to get an empty list for all the items from the left side that were not present in the right side. Calling `DefaultIfEmpty()` on the joined elements will return the joined items or the singleton list created from `default(Bar)` in case it was empty.

```C#
void Main() {
    var input1 = new List<Foo> { 
        new Foo(1, "A"), new Foo(2, "B"), new Foo(3, "C") 
    };
    var input2 = new List<Bar> { 
        new Bar(2, 200), new Bar(3, 300), new Bar(4, 400) 
    };
    LeftJoinWithLoop(input1, input2).Dump();
    LeftJoinWithLinq(input1, input2).Dump();
}

List<string> LeftJoinWithLoop(List<Foo> input1, List<Bar> input2) {
    var result = new List<string>();

    foreach (var item1 in input1) {
        var item2 = input2.Find(x => x.Id == item1.Id);
        result.Add($"{item1.Value}{item2?.Value}");
    }

    return result;
}

IEnumerable<string> LeftJoinWithLinq(List<Foo> input1, List<Bar> input2) => input1
    .GroupJoin(
        input2, 
        f => f.Id, 
        b => b.Id, 
        (f, bs) => (f, bs))
    .SelectMany(
        x => x.bs.DefaultIfEmpty(),
        (x, b) => $"{x.f.Value}{b?.Value}");
```

The LINQ solution looks a bit hacky but should be extracted into a reusable extension method anyway. The [MoreLINQ](https://github.com/morelinq/MoreLINQ) nuget package offers a `LeftJoin()` as well.

<a name="11"></a>
## 11. Right join

The right join works the same way as the left join only with the operands flipped. 

```C#
void Main() {
    var input1 = new List<Foo> { 
        new Foo(1, "A"), new Foo(2, "B"), new Foo(3, "C") 
    };
    var input2 = new List<Bar> { 
        new Bar(2, 200), new Bar(3, 300), new Bar(4, 400) 
    };
    RightJoinWithLoop(input1, input2).Dump();
    RightJoinWithLinq(input1, input2).Dump();
}

List<string> RightJoinWithLoop(List<Foo> input1, List<Bar> input2) {
    var result = new List<string>();

    foreach (var item2 in input2) {
        var item1 = input1.Find(x => x.Id == item2.Id);
        result.Add($"{item1?.Value}{item2.Value}");
    }

    return result;
}

IEnumerable<string> RightJoinWithLinq(List<Foo> input1, List<Bar> input2) => input2
    .GroupJoin(
        input1, 
        b => b.Id, 
        f => f.Id, 
        (b, fs) => (b, fs))
    .SelectMany(
        x => x.fs.DefaultIfEmpty(),
        (x, f) => $"{f?.Value}{x.b.Value}");
```

The LINQ implementation should also be generalized to work on any `IEnumerable<TLeft>` and `IEnumerable<TRight>` though. Or you could use the `RightJoin()` from MoreLINQ.

<a name="12"></a>
## 12. Full join

The full join is a beast. Period. Prepare for some serious head-scratching if you are ever faced with implementing it.

The naïve solution involves doing both the left and right join and combining the results while duplicates are being discarded. This is how the loop solution works.
   
The LINQ solution (stolen from https://stackoverflow.com/a/13503860/1025555) is a bit more clever. It creates `Lookup`s first which offer constant-time lookup speed of the ids. It's still not exactly beautiful and should be hidden by generalizing it. MoreLINQ has a `FullJoin()` as well.

```C#
void Main() {
    var input1 = new List<Foo> { 
        new Foo(1, "A"), new Foo(2, "B"), new Foo(3, "C") 
    };
    var input2 = new List<Bar> { 
        new Bar(2, 200), new Bar(3, 300), new Bar(4, 400) 
    };
    FullJoinWithLoop(input1, input2).Dump();
    FullJoinWithLinq(input1, input2).Dump();
}

List<string> FullJoinWithLoop(List<Foo> input1, List<Bar> input2) {
    var result = new HashSet<string>();

    foreach (var item1 in input1) {
        var item2 = input2.Find(x => x.Id == item1.Id);
        result.Add($"{item1.Value}{item2?.Value}");
    }

    foreach (var item2 in input2) {
        var item1 = input1.Find(x => x.Id == item2.Id);
        result.Add($"{item1?.Value}{item2.Value}");
    }

    return result.ToList();
}

IEnumerable<string> FullJoinWithLinq(List<Foo> input1, List<Bar> input2) {
    var lookup1 = input1.ToLookup(x => x.Id);
    var lookup2 = input2.ToLookup(x => x.Id);
    var ids = new HashSet<int>(lookup1.Select(p => p.Key));
    ids.UnionWith(lookup2.Select(p => p.Key));
    
    return ids
        .SelectMany(id => lookup1[id].DefaultIfEmpty(), (id, f) => (id, f))
        .SelectMany(x => lookup2[x.id].DefaultIfEmpty(), (x, b) => (x.f, b))
        .Select(x => $"{x.f?.Value}{x.b?.Value}");
}
```

<a name="13"></a>
## 13. Conditional join

So why did I spent so much time showing how to do the different kinds of joins with LINQ? The answer is simple: because that's what developers are doing more often than they might realize.

The problem is, in the real world requirements are not as "synthetic" as in the previous examples. Reality is often complex and it's hard to identify and abstract patterns when they are riddled with special cases and exceptions.

For instance, no client will come to you and request a plain inner join of `Foo`s and `Bar`s. It's more likely that he or she says you should "marry" every second `Foo` with all the `Bar`s that "belong to it". _But_ only the `Bar`s that are whole numbers!

"Easy enough", might Sir Loopalot think and begin hacking a nested loop solution right away. And after sprinkling a couple of `if`s here and there it even works! Done.

I say take a step back first. Think about the abstract nature of the problem. What kind of patterns are beneath the requirements? Can I split the process into several independent steps? What parts of my toolbox can I reuse? 

Of course, most developers will consider those questions before starting to code. But LINQ actually enforces this kind of thinking. LINQ makes it harder for you to take shortcuts that sacrifice maintainability on the long run.

```C#
void Main() {
    var input1 = new List<Foo> { 
        new Foo(1, "A"), new Foo(2, "B"), new Foo(3, "C"), new Foo(4, "D")
    };
    var input2 = new List<Bar> { 
        new Bar(1, 10), new Bar(2, 20.3), new Bar(3, 30.5), new Bar(4, 40)
    };
    ConditionalJoinWithLoop(input1, input2).Dump();
    ConditionalJoinWithLinq(input1, input2).Dump();
}

List<string> ConditionalJoinWithLoop(List<Foo> input1, List<Bar> input2) {
    var result = new List<string>();

    foreach (var item1 in input1) {
        if (item1.Id % 2 != 0)
            continue;
        
        foreach (var item2 in input2.FindAll(x => x.Id == item1.Id))
            if (item2.Value == Math.Floor(item2.Value))
                result.Add($"{item1.Value}{item2.Value}");
    }

    return result;
}

IEnumerable<string> ConditionalJoinWithLinq(List<Foo> input1, List<Bar> input2) => input1
    .Where(f => f.Id % 2 == 0)
    .Join(input2, f => f.Id, b => b.Id, (f, b) => (f, b))
    .Where(x => x.b.Value == Math.Floor(x.b.Value))
    .Select(x => $"{x.f.Value}{x.b.Value}");
```

The amount and the density of lambdas in the LINQ solution might be confusing at first, but that's just someting one has to learn and get used to. Look at it this way: understanding how some method solves a specific problem with nested `for`s and `if`s helps you with exactly one thing--understanding how this one method works. But learning `Join()` and how it can be used will help you everytime you see it in action somewhere else.

Nethertheless I will show how to beautify the above solution in [example 17](#17).

<a name="14"></a>
## 14. Reduce

Now we get to the mother of all LINQ operators. Quite Literally, because this operator could in fact be used to implement most of the operators I've shown you know so far. 

Enter `Aggregate()`. Probably the most dreaded LINQ operator there is, since it seems unwieldy and hard to wrap your head around. I know I had my problems at first.

But it really boils down to this: `Aggreagte()` reduces a list of things to a single thing with the operation you provide it. If the list might be empty or the type of the result is different from the type of the elements in the list, then you also have to give it a starting value (the _seed_).

```C#
void Main() {
    var input = new[] { 1, 2, 3 };
    ReduceWithLoop(input).Dump();
    ReduceWithLinq(input).Dump();
}

int ReduceWithLoop(int[] input) {
    var result = 0;

    foreach (var item in input)
        result += item;

    return result;
}

int ReduceWithLinq(int[] input) => input
    .Aggregate(0, (sum, next) => sum + next);
```

<a name="15"></a>
## 15. Moving window

What if operating on list elements independently is not sufficient? What if the element's neighbors need to be considered as well? 

Truth is there is no nice way of doing this with LINQ. If you know you are dealing with `IList<T>`s then you can use the trick demonstrated below. But if your `IEnumerable<T>` does not have an indexer then `ElementAt()` will go through all the elements until it reaches the `i`th one and performance will suffer.

Alternatively you can use MoreLINQ's `Pairwise()`. I added it's implementation here so you can see how it looks when LINQ reaches its limits.

If you have to resort to using the `IEnumerator<T>` of your `IEnumerable<T>` it's probably best that you generalize the code into a reusable extension method and hide it somewhere.

```C#
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
```

<a name="16"></a>
## 16. Tap

The tap operator is helpful when you have to hook side effects lazily into your operator chain.

Think of it as tapping a phone line: you can listen in on all elements going down the chain without anyone below noticing.

```C#
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
```

<a name="17"></a>
## 17. Modularization

What makes LINQ so versatile are the ways a LINQ chain can be modularized.

For one, you can replace complicated predicates and lambdas with private methods. You can also do that with the expression in an `if` statement, but the fact that LINQ operators take `Func`s allows you to elide those noisy arguments.

It's also possible to replace one or more operators of your chain with an extension method that reduces complexity and allows readers to focus on the important parts. Notice how the second LINQ example almost reads like what the client was requesting back in [example 13](#13).

```C#
void Main() {
    var input1 = new List<Foo> {
        new Foo(1, "A"), new Foo(2, "B"), new Foo(3, "C"), new Foo(4, "D")
    };
    var input2 = new List<Bar> {
        new Bar(1, 10), new Bar(2, 20.3), new Bar(3, 30.5), new Bar(4, 40)
    };
    ConditionalJoinWithLinq(input1, input2).Dump();
    ConditionalJoinWithBeautifulLinq(input1, input2).Dump();
}

IEnumerable<string> ConditionalJoinWithLinq(
    List<Foo> input1, List<Bar> input2) 
    => input1
        .Where(f => f.Id % 2 == 0)
        .Join(input2, f => f.Id, b => b.Id, (f, b) => (f, b))
        .Where(x => x.b.Value == Math.Floor(x.b.Value))
        .Select(x => $"{x.f.Value}{x.b.Value}");

IEnumerable<string> ConditionalJoinWithBeautifulLinq(
    List<Foo> input1, List<Bar> input2) 
    => input1
        .EverySecondFoo()
        .JoinBars(input2.Where(IsInteger))
        .Select(x => $"{x.f.Value}{x.b.Value}");

internal static class Extensions {
    public static IEnumerable<Foo> EverySecondFoo(this IEnumerable<Foo> foos) 
        => foos.Where(f => f.Id % 2 == 0);

    public static IEnumerable<(Foo f, Bar b)> JoinBars(
        this IEnumerable<Foo> foos, IEnumerable<Bar> bars) 
        => foos.Join(bars, f => f.Id, b => b.Id, (f, b) => (f, b));
}

bool IsInteger(Bar b) => b.Value == Math.Floor(b.Value);
```

Notice how we are using extensions methods in way that probably wasn't intended by the C# developers. Extensions methods have to be public, but here we only need them locally in our current context. The best we can do is to hide them in a deep namespace and make the extension class `internal`.

<a name="18"></a>
## 18. Laziness

What throws a lot of people off when starting with LINQ is it's laziness. Take the LINQ chain below for example. It increments each number in the array by one, squares them, and then prints them to the console using `Tap()`. But when you run the example in LINQPad nothing happens. There is no output whatsoever. Why? 

```
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
```

The answer is that LINQ operators only begin their work when something actually requests a value from them. This is called _deferred execution_: you can chain as many LINQ operators as you like and even pass that query around in a variable, but as long as no one does "something" with its values, nothing will be executed.

That "something" that has to be done is _enumeration_ and is usually initiated by a `foreach` loop iterating over the `IEnumerable` returned by a LINQ chain. Of course it doesn't have to be an explicit `for`/`foreach` loop. Some LINQ operators, like `ToList()` and our `ForEach()`, do the looping implicitly.

Enumerating the values of a LINQ chain triggers a process called _materialization_ where the last operator of the chain is queried for a value by the enumerating loop. This operator will in turn request a value from its input, which might be another operator preceeding it. The chain is traversed backwards up until an `IEnumerable` is reached that acts as the _value generator_ of the chain. Such a generator could be anything from an in-memory collection, a specialized generator function like `Enumerable.Range()`, the filesystem, a database query; you name it.

As soon as the first value was generated it will be fed to first operator in the chain, which will do its work and then forward it to the next operator. This continues until the value reaches the _enumerator_. When the enumerator requests the next value the same process will start over again.

If you check the implementation of `Tap()` now, you will notice that it does not do any looping at all. It just adds another `Select()` to the chain. That's why there is nothing happening. We canot use `Tap()` to terminate our chain, because it cannot act as an enumerator. We have to use `ForEach()`.

The rule of thumb is that an operator returning `IEnumerable<T>` will deferr execution and an operator returning `void` will enumerate.

Let's finish this section with a motivational picture that should help getting those lazy LINQ operators to work harder:

![image](https://user-images.githubusercontent.com/1454629/53301663-e3660380-3855-11e9-8316-58d447d7b33e.png)

<a name="summary"></a>
## 19. Summary

Let's summarize the advantages of LINQ we noticed in the previous examples.

- LINQ promotes reuse whereas code with loops tends to reinvent the wheel. If the compiler can do the looping for me then why should I bother to do it myself?
- The different LINQ operators have to be learned once, but every instance of a loop has to be understood on its own, because its effect is not always obvious and might even depend on minor implemenation details.
- LINQ enforces a recognizable pattern: split your problem into smaller operations and chain them to form a pipeline.
- LINQ is _declarative_ by nature and allows you to focus on intent instead of implemenation.
- The more complex a problem the more nesting will be required in a solution using loops. LINQ chains are usually not nested, but linear and can be extended easily.
- LINQ chains can be seamlessly modularized using extension functions.
- Some list operations (e.g. join) have performance traps when DIY'ed naïvely.
- `Func`s allow you to omit noisy parameters and arguments.

I hope that I could demonstrate how cleanly even complex logic can be organized with LINQ. I also hope you now share some of my obsessive hatred of loops :)
