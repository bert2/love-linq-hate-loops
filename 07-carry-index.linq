<Query Kind="Program" />

/* The previous examples were all fine and dandy, but this is were programmers
   used to looping begin to object.
   
   The example pairs each element index (the place of a runner in a competition)
   with the elements of a nested collection (the drugs the runner used). We are 
   _carrying_ the indeces down into a deeper level, so to speak. It's probably 
   easier to understand if you just run the example right now and have a look at 
   the output.
   
   Carrying the index from the outer loop into the inner loop's body is
   straightforward and doesn't look much different than any other nested loop. In
   LINQ however, this may not come as easy at first, because you explicitely have
   to "push" the index into the next operator.
   
   But what seems like a nuisance actually makes your life easier. Implicitely 
   being able to use variables from outer scopes also increases the amount of 
   information readers of your code have to keep track of. By explicitely 
   declaring what goes into the next operator, you are not only announcing what
   the important parts of the input data are. You are also keeping the scope of
   the following operator clean and tidy, making it easier to reason about.
   
   Don't try and boost your operator's scope by nesting lambdas like 
   `CarryIndexWithDirtyLinq()` does. This is 
   basically the same way the looping solution works and we want to do better 
   than that. */

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