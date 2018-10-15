<Query Kind="Program" />

/* The right join works the same way as the left join only with the operands
   flipped. 
   
   The LINQ implementation should also be generalized to work on any
   `IEnumerable<TLeft>` and `IEnumerable<TRight>` though. Or you could use the
   `RightJoin()` from MoreLINQ. */

void Main() {
    var input1 = new List<Foo> { 
        Foo.New(1, "A"), Foo.New(2, "B"), Foo.New(3, "C") 
    };
    var input2 = new List<Bar> { 
        Bar.New(2, 200), Bar.New(3, 300), Bar.New(4, 400) 
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
    public static Foo New(int id, string v) => new Foo { Id = id, Value = v };
    public int Id { get; set; }
    public string Value { get; set; }
}

class Bar {
    public static Bar New(int id, int v) => new Bar { Id = id, Value = v };
    public int Id { get; set; }
    public int Value { get; set; }
}