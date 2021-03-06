<Query Kind="Program" />

/* Flattening a list of lists is a fairly common operation. In loop land this is 
   done with nesting. In LINQtopia however, it's just a call to `SelectMany()`. 
   
   Note how this scales when applied to lists of lists of lists of... With loops 
   you will need even more _nesting_, whereas with LINQ you keep _chaining_ 
   `SelectMany()` until you reach the desired depth. */

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
