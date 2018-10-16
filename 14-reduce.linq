<Query Kind="Program" />

/* Now we get to the mother of all LINQ operators. Quite Literally, because this 
   operator could in fact be used to implement most of the operators I've shown 
   you know so far. 
   
   Enter `Aggregate()`. Probably the most dreaded LINQ operator there is, since
   it seems unwieldy and hard to wrap your head around. I know I had my problems
   at first.
   
   But it really boils down to this: `Aggreagte()` reduces a list of things to a 
   single thing with the operation you provide it. If the list might be empty or
   the type of the result is different from the type of the elements in the list,
   then you also have to give it a starting value (the _seed_). */

void Main() {
    var input = new[] { 1, 2, 3 };
    ReduceWithLoop(input).Dump();
    ReduceWithLinq(input).Dump();
}

int ReduceWithLoop(int[] input) {
    var result = 0;

    foreach (var item in input)
        result += item;

    return result;
}

int ReduceWithLinq(int[] input) => input
    .Aggregate(0, (sum, next) => sum + next);
