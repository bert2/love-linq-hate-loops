<Query Kind="Program" />

/* You might wonder now how an `IEnumerable` achieves this laziness. How can
   `Enumerable.Range()`, for instance, feed one value downstream and stop 
   executing until the next value is requested? How does it know where it left
   off earlier when the request for the next value comes in?
   
   On the syntax level this is done with the `yield` keyword. It can only be 
   used inside methods or properties that return an `IEnumerable`. When a value
   is requested from this member, its code will be run until the first
   `yield return <some value>` is hit. The value will be returned, the state of 
   all local variables will be saved, and execution stops until the next value is 
   requested. As soon as this happens, the local state will be restored and 
   execution will be picked up after the `yield return` that was executed last.
   
   Still sounds like magic? Maybe so, but at least we now know how to write lazy
   methods ourselves. We will layer have a brief look at how C# actually 
   implements this.
   
   Let's start by looking at the `Range()` method below. For simplicity we 
   iterate over the result of `Range()` with regular `foreach` loop. I added some
   debug outputs so its easier to see which statement gets executed when. Notice 
   how execution goes back and forth between the enumerator loop and the 
   generator method. Also notice how such asynchronous producer/consumer 
   scenarios usually had to be implemented with painful multi-threading and now 
   all we need is this little word `yield` and an `IEnumerable` return type!
   
   Now for something more glorious: the Fibonacci sequence! There is a minor 
   problem though: the first two Fibonacci numbers are predefined and not part of
   our computation loop. How do we deal with this? Simple! Just yield them
   before the computation loop. Those two `yield return` statements will never be
   hit again, because exectution will always continue _after_ the `yield return`
   executed last.
   
   But there's another problem: the Fibonacci sequence is infinite, but the range
   of `int` is not. And the numbers grow so fast that we cannot even get 50 
   values from our generator without getting overflown negative numbers. The 
   solution is to only return those numbers we can accurately compute and stop 
   the generator as soon as we overflow the `int` range. We do this by breaking 
   the generator loop almost like we would break a regular loop. We only have to 
   use `yield break` instead of `break`, because we are in an `IEnumerable` 
   context.
   
   You might want to get fancy and define your collections with static data the
   same way the property `MyHobbies` does it. Although it looks a tiny bit cleaner
   compared to `new`ing an array, you still shouldn't do this. The reason is that
   the compiler generates a whole lot of code for each member that uses `yield`.
   Every such member essentially becomes its own class capable of halting and 
   resuming execution while preserving local state (i.e. a state machine). You
   can see for yourself by opening a decompiled view of our LINQPad code with 
   `Alt`+`Shift`+`R` (requires ILSpy) and navigating to the compiler-generated 
   class `<get_MyHobbies>d__5`. */

void Main() {
    foreach (var num in Range(10, 13))
        Console.WriteLine($"got {num}");
    
    FibonacciNumbers.Take(50).Dump();
    
    MyHobbies.Dump();
}

public IEnumerable<int> Range(int start, int end) {
    for (var n = start; n <= end; n++) {
        Console.WriteLine($"yielding {n}");
        yield return n;
        Console.WriteLine("picking up work again...");
    }
}

public IEnumerable<int> FibonacciNumbers {
    get {
        var preprevious = 0;
        var previous = 1;
        yield return preprevious;
        yield return previous;
        
        while (true) {
            var current = preprevious + previous;
            
            if (current < 0) yield break;
            
            yield return current;
            preprevious = previous;
            previous = current;            
        }
    }
}

public IEnumerable<string> MyHobbies {
    get { 
        yield return "horses";
        yield return "flowers";
        yield return "guns";
    }
}