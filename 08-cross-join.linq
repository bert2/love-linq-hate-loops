<Query Kind="Program" />

/* When you see nested loops working away on two collections, chances are they 
   are both being joined somehow. The question is: what kind of join you are 
   looking at?
   
   When done with loops it's not obvious. Often a develooper (sorry) will 
   "invent" his or her own logic that looks like someone was trying to build the 
   death star. This kind of DIY joining does nothing but obfuscate intent and 
   makes your reviewer question your mental stability.
   
   Always figure out what kind of join your code should perform _before_ you 
   start coding. Then express this operation in abstract terms (cross, inner, 
   left, right, full). Those are concepts that your readers can google if they 
   don't know them.
   
   The simplest kind of join is the cross join (a.k.a. cartesian product):
   everything is paired with everthing. In LINQ it's just a call to 
   `SelectMany()`. */

void Main() {
    var input1 = new[] { 'A', 'B', 'C' };
    var input2 = new[] { 1, 2, 3 };
    CrossJoinWithLoop(input1, input2).Dump();
    CrossJoinWithLinq(input1, input2).Dump();
}

List<string> CrossJoinWithLoop(char[] input1, int[] input2) {
    var result = new List<string>();

    foreach (var item1 in input1) {
        foreach (var item2 in input2)
            result.Add($"{item1}{item2}");
    }
    
    return result;
}

IEnumerable<string> CrossJoinWithLinq(char[] input1, int[] input2) => input1
    .SelectMany(_ => input2, (x1, x2) => $"{x1}{x2}");
