using System;
using Antlr4.Runtime.Misc;
// This is a visitor used to obtain a "xor" expression composed with operator "^"
public class XorExprVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public XorExprVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitXor_expr([NotNull] Python3Parser.Xor_exprContext context)
    {
        result = new LineModel();
        // If there is one child then it is an and_expr.
        if (context.ChildCount == 1)
        {
            AndExprVisitor newVisitor = new AndExprVisitor(state);
            context.GetChild(0).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        // If there is more than one child, then we have the following children:
        // Child #0: expr_1
        // Child #1: "^"
        // Child #2: expr_2
        // (Child #3: "^"
        //  Child #4: expr_3
        //  ... ) - optional
        else if (context.ChildCount > 1)
        {
            AndExprVisitor firstVisitor = new AndExprVisitor(state);
            context.GetChild(0).Accept(firstVisitor);
            for (int j = 0; j < firstVisitor.result.tokens.Count; ++j)
            {
                result.tokens.Add(firstVisitor.result.tokens[j]);
            }
            int i = 1;
            int n = context.ChildCount;
            while (i + 1 < n)
            {
                // "^" operator
                result.tokens.Add(context.GetChild(i).ToString());
                // shift_expr
                AndExprVisitor newVisitor = new AndExprVisitor(state);
                context.GetChild(i + 1).Accept(newVisitor);
                for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
                {
                    result.tokens.Add(newVisitor.result.tokens[j]);
                }
                i += 2;
            }

        }
        return result;
    }


}