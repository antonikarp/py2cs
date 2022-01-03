using System;
using Antlr4.Runtime.Misc;

public class ClassdefVisitor : Python3ParserBaseVisitor<Class>
{
    public Class result;
    public State state;
    public ClassdefVisitor(State _state)
    {
        state = _state;
    }
    public override Class VisitClassdef([NotNull] Python3Parser.ClassdefContext context)
    {
        // We assume that we have the following children (so far no inheritance):

        // Child #0: "class"
        // Child #1: class name
        // Child #2: ":"
        // Child #3: suite

        result = new Class(state.output);
        result.name = context.GetChild(1).ToString();
        state.output.currentClasses.Push(result);
        
        SuiteVisitor suiteVisitor = new SuiteVisitor(state);
        context.suite().Accept(suiteVisitor);

        result = state.output.currentClasses.Pop();
        // If the function is not internal, add it to the list of classes in
        // the output.
        // Otherwise, add it to the list of classes in the current class.
        // Remember that at the bottom there is always the Program class.
        if (state.output.currentClasses.Count > 1)
        {
            Class parentClass = state.output.currentClasses.Peek();
            parentClass.internalClasses.Add(result);
        }
        else
        {
            state.output.classes.Add(result);
        }
        state.output.allClasses.Add(result);

        return result;
    }

}
