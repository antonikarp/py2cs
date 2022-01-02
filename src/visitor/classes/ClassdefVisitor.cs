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
        
        // So far no nested classes.
        state.output.classes.Add(result);
        return result;
    }

}
