using System;
using Antlr4.Runtime.Misc;

// This is a visitor to be used to compute an arithmetic expression.
// It traverses the parse tree from the node 'arith_expr'.
public class ArithExprVisitor : Python3ParserBaseVisitor<ArithExpr>
{
    public ArithExpr result;
    public override ArithExpr VisitArith_expr([NotNull] Python3Parser.Arith_exprContext context)
    {
        result = new ArithExpr();
        for (int i = 0; i < context.ChildCount; ++i)
        {
            if (context.GetChild(i).ToString() == "+")
            {
                result.tokens.Add("+");
            }
            else if (context.GetChild(i).ToString() == "-")
            {
                result.tokens.Add("-");
            }
            else // We have encountered a term.
            {
                TermVisitor newVisitor = new TermVisitor();
                context.GetChild(i).Accept(newVisitor);
                for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
                {
                    result.tokens.Add(newVisitor.result.tokens[j]);
                }
            }
        }
        return result;
    }
}
