<Query Kind="Program" />

/* The inner join is also a fairly simple join. It's basically the intersection of
   two lists based on a common key. 
   
   The inner join is so prevalent that LINQ has it's own operator for it: 
   `Join()`. There is no excuse not to use it. Especially considering that the 
   inner join is often implemented rather ineffeciently when done by looplanders.
   Can you spot the performance issue in the loop example? 
   
   The nested loops look innocent, but we do a linear search there in order to
   find the matching item in `input2`. Madness! That's `O(NM)`! Converting
   `input2` to a `Lookup<Bar>` beforehand will give us `O(N)`.
   
   Or we can just use `Join()` and let LINQ do the busywork for us... */

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