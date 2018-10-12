<Query Kind="Program" />

/* Sometimes you need the element index in your mapping function. Sounds like
   the perfect job for a classic for loop, you think? Not on my watch! 
   
   LINQ operators usually have overloads that feed the element along with its 
   index to your Func if you need them.
   
   The example below maps the bits of a binary number to their decimal
   representations. Using the index of the bits this is easy enough, but
   requires us to iterate over them backwards. Of course this can be done with 
   a for loop (after fixing this damn off-by-one error... again), but see how
   natural you can express this reversal with LINQ. */

void Main() {
    var input = new[] { 1, 1, 0, 1 };

    var result1 = CaptureIndexWithLoop(input);
    result1.Dump();

    var result2 = CaptureIndexWithLinq(input);
    result2.Dump();
}

List<int> CaptureIndexWithLoop(int[] input) {
    var result = new List<int>();

    for (var i = input.Length - 1; i >= 0; i--) {
        result.Add(input[i] << i);
    }

    return result;
}

IEnumerable<int> CaptureIndexWithLinq(int[] input) =>
    throw new NotImplementedException();