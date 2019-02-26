<Query Kind="Program" />

/* The right join works the same way as the left join only with the operands
   flipped. 
   
   The LINQ implementation should also be generalized to work on any
   `IEnumerable<TLeft>` and `IEnumerable<TRight>` though. Or you could use the
   `RightJoin()` from MoreLINQ. */

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
