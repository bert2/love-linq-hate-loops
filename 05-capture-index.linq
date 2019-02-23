<Query Kind="Program" />

/* Sometimes you need the element index in your mapping function. Sounds like the
   perfect job for a classic `for` loop, you think? Not on my watch! 
   
   LINQ operators usually have overloads that feed the element along with its 
   index to your Func if you need them.
   
   The example below converts a binary number to decimal. Using the index of the 
   bits this is easy enough, but we have to keep in mind that the least 
   significant bit is the _last_ element of the input. Of course we can work out 
   the correct process with a `for` loop (after fixing this damn off-by-one 
   error...), but see how natural you can express this reversal with LINQ.
   
   Notice how the parameters of the lambda in `Select((x, i) => ...`) match up 
   with parameters of `MulByNthPowerOfTwo(int x, int n)`? This means we could
   leave them out entirely and write `Select(MulByNthPowerOfTwo)`, which is less 
   redundant. */

void Main() {
    var input = new[] { 1, 1, 0, 1 };
    CaptureIndexWithLoop(input).Dump();
    CaptureIndexWithLinq(input).Dump();
}

int CaptureIndexWithLoop(int[] input) {
    var result = 0;
    var lastIndex = input.Length - 1;
    
    for (var i = 0; i < input.Length; i++)
        result += MulByNthPowerOfTwo(input[i], lastIndex - i);

    return result;
}

int CaptureIndexWithLinq(int[] input) => input
    .Reverse()
    .Select((x, i) => MulByNthPowerOfTwo(x, i))
    .Sum();

int MulByNthPowerOfTwo(int x, int n) => x << n;