<Query Kind="Program" />

/* The only acceptable for loop is the one that has only one statement in it's
   body and does not need curly braces. In fact it should be enforced that
   loops are defined without curly braces to make sure they always have
   single-statement bodies.
   
   Make the loop body a method in case it needs more statements. All dependencies
   should be parameters of that method.
   
   Still a for loop should only be used when the performance gain due to in-place
   editing of collections really is needed. */

void Main() {
    var foos = new[] { new Foo(), null, new Foo() };
    EnsureAllInitialized(foos);
    foos.Dump();
}

void EnsureAllInitialized(Foo[] foos) {
    for (var i = 0; i < foos.Length; i++)
        foos[i] = foos[i] ?? new Foo();
}

class Foo { }
