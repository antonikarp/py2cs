using System;
using Antlr4.Runtime.Misc;

// This is a visitor used to compute a "shift" expression composed with operators
// "<<" and ">>". It traverses the parse tree from the node "shift_expr".
public class ShiftExprVisitor : Python3ParserBaseVisitor<ShiftExpr>
{
    public ShiftExpr result;
    public override ShiftExpr VisitShift_expr([NotNull] Python3Parser.Shift_exprContext context)
    {
        result = new ShiftExpr();
        // If there is one child then it is an arithmetic expression.
        if (context.ChildCount == 1)
        {
            ArithExprVisitor newVisitor = new ArithExprVisitor();
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
            ArithExprVisitor leftVisitor = new ArithExprVisitor();
            ArithExprVisitor rightVisitor = new ArithExprVisitor();
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