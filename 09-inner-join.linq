<Query Kind="Program" />

/* The inner join also is a fairly simple join. It's basically the intersection of
   two lists based on a common key. 
   
   The inner join is so prevalent that LINQ has it's own operator for it: 
   `Join()`. There is no excuse not to use it. Especially considering that the 
   inner join is often implemented rather ineffeciently when done by looplanders.
   Can you spot the performance issue in the loop example? 
   
   The nested loops look innocent, but we do a linear search there in order to
   find the matching item in `input2`. Madness! That's `O(NM)`! Converting
   `input2` to a `Lookup<Bar>` beforehand would give us `O(N)`.
   
   Or we can just use `Join()` and let LINQ do the busywork for us... */

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