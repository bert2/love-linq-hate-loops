<Query Kind="Program" />

/* What makes LINQ so versatile are the ways a LINQ chain can be modularized. 

   For one, you can replace complicated predicates and lambdas with private 
   methods. You can also do that with the expression in an `if` statement, but
   the fact that LINQ operators take `Func`s allows you to elide those noisy 
   arguments.
   
   It's also possible to replace one or more operators of your chain with an 
   extension method that reduces complexity and allows readers to focus on the 
   important parts. Notice how the second LINQ example almost reads like what the
   client was requesting back in example 13.
   
   Notice how we are using extensions methods in way that probably wasn't 
   intended by the C# developers. Extensions methods have to be public, but here 
   we only need them locally in our current context. The best we can do is to 
   hide them in a deep namespace and make the extension class `internal`. */

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

class Foo {
    public Foo(int id, string v) { Id = id; Value = v; }
    public int Id { get; set; }
    public string Value { get; set; }
}

class Bar {
    public Bar(int id, double v) { Id = id; Value = v; }
    public int Id { get; set; }
    public double Value { get; set; }
}
