<Query Kind="Program" />

/* So why did I spent so much time showing how to do the different kinds of joins
   with LINQ? The answer is simple: because that's what developers are doing more
   often than they might realize.
   
   The problem is, in the real world requirements are not as "synthetic" as in 
   the previous examples. Reality is often complex and it's hard to identify and
   abstract patterns when they are when they are riddled with special cases and 
   exceptions.
   
   For instance, no client will come to you and request a plain inner join of 
   `Foo`s and `Bar`s. It's more likely that he or she says you should "marry" 
   every second `Foo` with all the `Bar`s that "belong to it". _But_ only the 
   `Bar`s that are whole numbers!
   
   "Easy enough", might Sir Loopalot think and begin hacking a nested loop 
   solution right away. And after sprinkling a couple of `if`s here and there it 
   even works! Done.
   
   I say take a step back first. Think about the abstract nature of the problem.
   What kind of patterns are beneath the requirements? Can I split the process 
   into several independent steps? What parts of my toolbox can I reuse? 
   
   Of course, most developers will consider those questions before starting to
   code. But LINQ actually enforces this kind of thinking. LINQ makes it harder
   for you to take shortcuts that sacrifice maintainability on the long run. */

void Main() {
    var input1 = new List<Foo> { 
        Foo.New(1, "A"), Foo.New(2, "B"), Foo.New(3, "C"), Foo.New(4, "D")
    };
    var input2 = new List<Bar> { 
        Bar.New(1, 10), Bar.New(2, 20.3), Bar.New(3, 30.5), Bar.New(4, 40)
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

class Foo {
    public static Foo New(int id, string v) => new Foo { Id = id, Value = v };
    public int Id { get; set; }
    public string Value { get; set; }
}

class Bar {
    public static Bar New(int id, double v) => new Bar { Id = id, Value = v };
    public int Id { get; set; }
    public double Value { get; set; }
}