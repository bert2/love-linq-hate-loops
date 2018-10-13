<Query Kind="Program" />

/* This is your bread... */

void Main() {
    var input = new[] { "1", "2", "3" };
    MapWithLoop(input).Dump();
    MapWithLinq(input).Dump();
}

List<int> MapWithLoop(string[] input) {
    var result = new List<int>();
    
    foreach (var item in input)
        result.Add(int.Parse(item));
    
    return result;
}

IEnumerable<int> MapWithLinq(string[] input) => 
    input.Select(int.Parse);
