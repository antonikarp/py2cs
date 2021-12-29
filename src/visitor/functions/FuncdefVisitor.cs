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

        TypedargslistVisitor parameterVisitor = new TypedargslistVisitor(state);
        context.GetChild(2).Accept(parameterVisitor);

        SuiteVisitor suiteVisitor = new SuiteVisitor(state);
        context.GetChild(4).Accept(suiteVisitor);

        // Commit a new function to the list of functions in the class state.
        result.name = context.GetChild(1).ToString();
        result.statements.lines = suiteVisitor.result.lines;
        result.isVoid = state.funcState.isVoid;
        result.parameters = state.funcState.parameters;
        result.isStatic = state.funcState.isStatic;
        result.defaultParameters = state.funcState.defaultParameters;
        state.classState.functions.Add(result);
        return result;
    }


}