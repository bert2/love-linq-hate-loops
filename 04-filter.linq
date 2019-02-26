<Query Kind="Program" />

/* ...and `Where()` is your butter. */

void Main() {
    var input = new[] { 1, 2, 3, 4 };
    FilterWithLoop(input).Dump();
    FilterWithLinq(input).Dump();
}

List<int> FilterWithLoop(int[] input) {
    var result = new List<int>();

    foreach (var item in input)
        if (item % 2 == 0)
            result.Add(item);

    return result;
}

IEnumerable<int> FilterWithLinq(int[] input) => input
    .Where(x => x % 2 == 0);
