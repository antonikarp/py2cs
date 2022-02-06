using System;
using Antlr4.Runtime.Misc;

public class TrailerConstructorCheckVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public bool isConstructor;
    public TrailerConstructorCheckVisitor(State _state)
    {
        isConstructor = true;
        state = _state;
    }
    public override LineModel VisitTrailer([NotNull] Python3Parser.TrailerContext context)
    {
        result = new LineModel();
        // After encoutering the first pair of parentheses, we don't set isConstructor to false.
        // This is to properly recognize such expressions:
        // B().fun() <- a constructor call
        // B.fun() <- not a constructor call
        if ((context.ChildCount == 2 && context.GetChild(0).ToString() == "(" && context.GetChild(1).ToString() == ")") ||
            (context.ChildCount == 3 && context.GetChild(0).ToString() == "(" && context.GetChild(2).ToString() == ")"))
        {
            state.trailerConstructorCheckState.isLocked = true;
        }
        else if (context.ChildCount == 2 && context.NAME() != null)
        {
            if (!state.output.allClassesNames.Contains(context.NAME().ToString()) && !state.trailerConstructorCheckState.isLocked)
            {
                isConstructor = false;
            }
        }
        return result;
    }

}