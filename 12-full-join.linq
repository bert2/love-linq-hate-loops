<Query Kind="Program" />

/* The full join is a beast. Period. Prepare for some serious head-scratching if
   you are ever faced with implementing it. 
   
   The naïve solution involves doing both the left and right join and combining 
   the results while duplicates are being discarded. This is how the loop solution
   works.
   
   The LINQ solution (stolen from https://stackoverflow.com/a/13503860/1025555) 
   is a bit more clever. It creates `Lookup`s first which offer constant-time 
   lookup speed of the ids. It's still not exactly beautiful and should be
   hidden by generalizing it. MoreLINQ has a `FullJoin()` as well. */

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