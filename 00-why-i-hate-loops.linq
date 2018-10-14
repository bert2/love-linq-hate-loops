<Query Kind="Statements" />

/* Can you see what the for loop below does? No? Me neither. This just goes to show
   what ugly kind of things are possible with a `for`/`foreach` loop.
   
   Of course, it's also possible to write clean code using `for` loops, as well it 
   is possible to write dirty code using LINQ. However, one of the two promotes 
   abstraction, modularization, and immutability more than the other. Guess which 
   one I'm talking about. 
   
   Have you ever considered the level of abstraction loops offer? It's actually 
   astonishingly low. In essence they are just syntactic sugar for a jump and a 
   branch instruction, which both wrap a couple more instructions. Sure, it's 
   efficient. But so is assembler. */

var a = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

var x = 0;

for (var i = 0; i < a.Length; i++) {
    if (i % 3 == 0 && i > 0)
        a[i - 1] += a[i];
    
    if (a[i] % 2 == 0) {
        --x;
        continue;
    } 
    
    a[i] += x;
}

a.Dump();