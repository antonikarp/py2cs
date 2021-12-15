using System;
using System.Text;
using Antlr4.Runtime.Misc;
public class TypedargslistVisitor : Python3ParserBaseVisitor<Empty>
{
    public Empty result;
    public State state;

    public TypedargslistVisitor(State _state)
    {
        state = _state;
    }
    public override Empty VisitTypedargslist([NotNull] Python3Parser.TypedargslistContext context)
    {
        result = new Empty();

        // We assume that we have the following children:
        // Child #0: tfpdef_0
        // Child #1: ","
        // Child #2: tfpdef_1
        // Child #2: ","
        // ...
        int n = context.ChildCount;
        int i = 0;
        while (i < n)
        {
            TfpdefVisitor newVisitor = new TfpdefVisitor(state);
            context.GetChild(i).Accept(newVisitor);
            state.funcState.parameters.Add(newVisitor.result.value);
            i += 2;
        }
        return result;
    }


}