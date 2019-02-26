<Query Kind="Statements" />

/* Can you see what the `for` loop below does? No? Me neither. This just goes to 
   show what ugly kind of things are possible with loops.
   
   Of course, it's also possible to write clean code using `for` loops, as well as
   it is possible to write dirty code using LINQ. However, LINQ promotes
   abstraction, modularization, and immutability more than loops do. LINQ does this
   by taking power away from you and in the beginning it might feel you are 
   programming with your hands tied to your back. But in the long run you will end 
   up with code that is easier to maintain.
   
   Have you ever considered the level of abstraction loops offer? It's actually 
   astonishingly low. In essence they are just syntactic sugar for an increment and
   a branch instruction that both wrap the instructions of your loop body. Sure, 
   it's efficient. But so is coding in assembly language. Because loops are so 
   low-level, some people even say that "the `for` loop will be the new `goto`" [1].
   
   The lack of abstraction of loops forces readers to carefully track state in 
   order to understand what your loop is doing. One cannot immediately tell whether
   the loop is for instance filtering or mapping the input list (or maybe it's 
   doing both operations at once). With LINQ operators intent becomes more 
   explicit, because a `Where()` will always filter and a `Select()` will always 
   map.
   
   The hard truth is though that most programmers, when given a problem that 
   requires some kind of looping, will be fastest when coding it using 
   `for`/`foreach` loops. I think that's because they already have an 
   _abstract model_ of the problem in their heads and coding appears as the act of 
   merely turning this model into _concrete instructions_. But that's where they 
   are wrong. Because now it is up to the readers of their code to 
   _reverse engineer_ that abstract model from a set of statements that really 
   could do anything.
   
   This is were LINQ helps. Using LINQ forces you to stay in the abstract realm 
   more than loops do and delegates the dirty business with concrete instructions 
   to the compiler.
   
   In short: excessively relying on loops is like not writing tests. Sure, you will
   be faster now, but in the end your colleagues will hate you for it.
   
   [1] https://www.quora.com/Can-we-say-that-the-for-loop-will-be-the-new-goto-%E2%80%9D-given-the-increasing-popularity-of-functional-programming/answer/Tikhon-Jelvis?share=1&srid=3SJB */

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
