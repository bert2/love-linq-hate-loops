<Query Kind="Program" />

/* The left join is a bit more tricky to do with LINQ, because there is no
   inherent support for it.
   
   Luckily we can use `GroupJoin()` to get an empty list for all the items from 
   the left side that were not present in the right side. Calling 
   `DefaultIfEmpty()` on the joined elements will return the joined items
   or the singleton list created from `default(Bar)` in case it was empty.
   
   The LINQ solution looks a bit hacky but should be extracted into a reusable
   extension method anyway. The MoreLINQ [1] nuget package offers a `LeftJoin()`
   as well. 
   
   [1] https://github.com/morelinq/MoreLINQ */

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