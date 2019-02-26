<Query Kind="Program" />

/* Sooner or later after you started writing more LINQ chains you will have to 
   debug them now and then. You will notice that things work a bit different than
   you are used to from regular imperative code with loops.
   
   You cannot set a break point on an operator and check how the input data as a 
   whole has been transformed so far. All you can do is break inside an 
   operator's lambda and check each item individually. So you will probably start 
   splitting the LINQ chain into smaller chains writing to temporary lists in 
   order to inspect the data there. This ends up in a tedious back and forth
   between changing the code and running the debugger until you find the bug. And
   after that you will have to change it all back again!
   
   I claim that it's not LINQ that is at fault here; it's your habits. Think about
   it: all a LINQ chain does is combine well-defined operators which are in 
   itself almost quaranteed to work correctly. Typical off-by-one index errors
   cannot happen, because we don't maintain the indeces ourselves. State variables
   shouldn't be subject to unexpected changes, because ideally our LINQ chains
   work in an immutable fashion (i.e. they do not change items, but yield new 
   ones instead).
   
   The bug will most likely be in our assumptions. Is the input data really shaped
   the way we think it is? Did we understand all the operators correctly? If we
   wrote a custom operator for our chain: is it covered by unit tests?
   
   In my experience you will spend a lot less time stepping through the code with
   a debbugger (which should always be a last resort anyway). Instead you will 
   spend more time just looking at your chain and checking whether the operators 
   are combined correctly.
   
   That being said: if you really have the need to see the complete data inside 
   the `Enumerable` after an arbitrary step in you chain, you can use the 
   `TapAll()` operator defined below. It's tap action does not take an individual
   item, but all items instead. Use it with a dummy action where you can place a 
   break point (e.g. `TapAll(x => {; })`) to inspect the data inside the debug 
   watch. */

void Main() {
    new[] { 1, 2, 3 }
        .TapAll(x => {})
        .Select(x => x + 1)
        .TapAll(x => {})
        .Select(x => x * x)
        .TapAll(x => {})
        .Dump();
}

public static class Extensions {
    public static IEnumerable<T> TapAll<T>(
        this IEnumerable<T> source, Action<IEnumerable<T>> action) {
        action(source);
        return source;
    }
}