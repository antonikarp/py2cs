using System;
using Antlr4.Runtime.Misc;

// This is a visitor used to compute a "shift" expression composed with operators
// "<<" and ">>". It traverses the parse tree from the node "shift_expr".
public class ShiftExprVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public ShiftExprVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitShift_expr([NotNull] Python3Parser.Shift_exprContext context)
    {
        result = new LineModel();
        // If there is one child then it is an arithmetic expression.
        if (context.ChildCount == 1)
        {
            ArithExprVisitor newVisitor = new ArithExprVisitor(state);
            context.GetChild(0).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        // If there are 3 children then we have an expression:
        // "<expr_1> << <expr_2>"
        // or "<expr_1> >> <expr_2>"
        else if (context.ChildCount == 3)
        {
            // Expression is standalone:
            if (!state.stmtState.isLocked)
            {
                state.stmtState.isStandalone = true;
                state.stmtState.isLocked = true;
            }
            ArithExprVisitor leftVisitor = new ArithExprVisitor(state);
            ArithExprVisitor rightVisitor = new ArithExprVisitor(state);
            context.GetChild(0).Accept(leftVisitor);
            context.GetChild(2).Accept(rightVisitor);
            for (int i = 0; i < leftVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(leftVisitor.result.tokens[i]);
            }
            result.tokens.Add(context.GetChild(1).ToString());
            for (int i = 0; i < rightVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(rightVisitor.result.tokens[i]);
            }
        }
        return result;
    }


}