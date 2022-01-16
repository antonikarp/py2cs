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
        state.output.currentClasses.Peek().currentFunctions.Push(new Function(state.output)); 

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

        result = state.output.currentClasses.Peek().currentFunctions.Pop();

        result.name = context.GetChild(1).ToString();

        
        // If the first paramter is "self" then we remove it.
        if (result.parameters.Count > 0 && result.parameters[0] == "self")
        {
            result.parameters.RemoveAt(0);
        }

        // Special case __init__ - constructor
        if (result.name == "__init__")
        {
            HandleInitMethod();
        }


        result.statements.lines = suiteVisitor.result.lines;

        // If the function is not internal, add it to the list of functions
        // in the class state.
        // Otherwise, add it to the list of functions in the current function.
        // If we are in the Program class, at the bottom there is always a Main
        // function.
        if (state.output.currentClasses.Peek().currentFunctions.Count > 1)
        {
            Function parentFunction = state.output.currentClasses.Peek().currentFunctions.Peek();

            // To be able to use parameters from the parent function.
            result.isStatic = false;

            parentFunction.internalFunctions.Add(result);
        }
        else
        {
            state.output.currentClasses.Peek().functions.Add(result);
        }
        result.parentClass = state.output.currentClasses.Peek();
        return result;
    }
    public void HandleInitMethod()
    {
        // Change the name to the name of the class (constructor).
        result.name = state.output.currentClasses.Peek().name;
        result.isConstructor = true;
        state.output.currentClasses.Peek().constructorSignatures
            [result.parameters.Count] = result;
    }


}