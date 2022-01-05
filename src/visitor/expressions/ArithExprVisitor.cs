using System;
using Antlr4.Runtime.Misc;

// This is a visitor to be used to compute an arithmetic expression.
// It traverses the parse tree from the node 'arith_expr'.
public class ArithExprVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public ArithExprVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitArith_expr([NotNull] Python3Parser.Arith_exprContext context)
    {
        result = new LineModel();
        // If there is one child then it is a term.
        if (context.ChildCount == 1)
        {
            TermVisitor newVisitor = new TermVisitor(state);
            context.GetChild(0).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        // If there is more than one child then we have the following children:
        // Child #0: term
        // Child #1: "+" or "-"
        // Child #2: term
        // ...
        else if (context.ChildCount > 1)
        {
            // Expression is standalone:
            if (!state.stmtState.isLocked)
            {
                state.stmtState.isStandalone = true;
                state.stmtState.isLocked = true;
            }
            int n = context.ChildCount;
            TermVisitor firstVisitor = new TermVisitor(state);
            context.GetChild(0).Accept(firstVisitor);
            for (int j = 0; j < firstVisitor.result.tokens.Count; ++j)
            {
                result.tokens.Add(firstVisitor.result.tokens[j]);
            }
            int i = 1;
            while (i + 1 < n)
            {
                if (context.GetChild(i).ToString() == "+")
                {
                    result.tokens.Add("+");
                }
                else if (context.GetChild(i).ToString() == "-")
                {
                    result.tokens.Add("-");
                }
                TermVisitor newVisitor = new TermVisitor(state);
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
