using System;
using System.Text;
using Antlr4.Runtime.Misc;
public class FuncdefVisitor : Python3ParserBaseVisitor<Function>
{
    public Function result;
    public State state;
    public FuncdefVisitor(State _state)
    {
        state = _state;
    }
    public override Function VisitFuncdef([NotNull] Python3Parser.FuncdefContext context)
    {
        result = new Function();
        state.funcState = new FuncState();
        // We assume that we have the following children:

        // Child 0: "def"
        // Child 1: function name
        // Child 2: parameters
        // Child 3: ":"
        // Child 4: suite

        result.name = context.GetChild(1).ToString();
        SuiteVisitor newVisitor = new SuiteVisitor(state);
        context.GetChild(4).Accept(newVisitor);
        result.statements.lines = newVisitor.result.lines;
        result.isVoid = state.funcState.isVoid;
        state.classState.functions.Add(result);
        return result;
    }


}