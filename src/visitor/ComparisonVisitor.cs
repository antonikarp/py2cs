using System;
using Antlr4.Runtime.Misc;

// This is a visitor used to compute a "shift" expression composed with operators
// "<<" and ">>". It traverses the parse tree from the node "shift_expr".
public class ComparisonVisitor : Python3ParserBaseVisitor<Comparison>
{
    public Comparison result;
    public override Comparison VisitComparison([NotNull] Python3Parser.ComparisonContext context)
    {
        result = new Comparison();
        // If there is one child then it is shift expression.
        if (context.ChildCount == 1)
        {
            ShiftExprVisitor newVisitor = new ShiftExprVisitor();
            context.GetChild(0).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        // If there are 3 children then we have 2 expressions joined by
        // a comparison operator (<, >, <=, >=, ==, !=)
        else if (context.ChildCount == 3)
        {
            ShiftExprVisitor leftVisitor = new ShiftExprVisitor();
            ShiftExprVisitor rightVisitor = new ShiftExprVisitor();
            context.GetChild(0).Accept(leftVisitor);
            context.GetChild(2).Accept(rightVisitor);
            for (int i = 0; i < leftVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(leftVisitor.result.tokens[i]);
            }
            CompOpVisitor opVisitor = new CompOpVisitor();
            context.GetChild(1).Accept(opVisitor);
            result.tokens.Add(opVisitor.result.value);
            for (int i = 0; i < rightVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(rightVisitor.result.tokens[i]);
            }
        }
        return result;
    }

}