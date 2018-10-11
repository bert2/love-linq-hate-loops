<Query Kind="Program" />

void Main() {
    var input = new[] {"1", "2", "3"};
    
    var nums1 = MapWithLoop(input);
    nums1.Dump();
    
    var nums2 = MapWithLinq(input);
    nums2.Dump();
}

List<int> MapWithLoop(string[] input) {
    var result = new List<int>();
    
    foreach (var item in input)
        result.Add(int.Parse(item));
    
    return result;
}
