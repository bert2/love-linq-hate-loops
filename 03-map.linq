<Query Kind="Program" />

/* This is your bread... */

void Main() {
    var input = new[] { "1", "2", "3" };
    
    var result1 = MapWithLoop(input);
    result1.Dump();
    
    var result2 = MapWithLinq(input);
    result2.Dump();
}

List<int> MapWithLoop(string[] input) {
    var result = new List<int>();
    
    foreach (var item in input)
        result.Add(int.Parse(item));
    
    return result;
}

IEnumerable<int> MapWithLinq(string[] input) =>
    throw new NotImplementedException();