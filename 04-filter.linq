<Query Kind="Program" />

/* ...and this is your butter. */

void Main() {
    var input = new[] { 1, 2, 3, 4 };

    var result1 = FilterWithLoop(input);
    result1.Dump();

    var result2 = FilterWithLinq(input);
    result2.Dump();
}

List<int> FilterWithLoop(int[] input) {
    var result = new List<int>();

    foreach (var item in input) {
        if (item % 2 == 0)
            result.Add(item);
    }

    return result;
}

IEnumerable<int> FilterWithLinq(int[] input) => 
    input.Where(x => x % 2 == 0);
