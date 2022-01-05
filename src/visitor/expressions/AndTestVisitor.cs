using System;
using Antlr4.Runtime.Misc;

// This is a visitor used to compute an expression created with a logical operator "and".
// It traverses the parse tree from the node "and_test".

public class AndTestVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public AndTestVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitAnd_test([NotNull] Python3Parser.And_testContext context)
    {
        result = new LineModel();
        // If there is one child then it is a 'not_test' node.
        if (context.ChildCount == 1)
        {
            NotTestVisitor newVisitor = new NotTestVisitor(state);
            context.GetChild(0).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        // If there are more than one child then we have the following children:
        // Child #0: <expr1>
        // Child #1: and
        // Child #2: <expr2>
        // Child #3: ...
        else if (context.ChildCount > 1)
        {
            // Expression is standalone:
            if (!state.stmtState.isLocked)
            {
                state.stmtState.isStandalone = true;
                state.stmtState.isLocked = true;
            }
            int n = context.ChildCount;
            int i = 0;
            while (i < n)
            {
                if (i != 0)
                {
                    result.tokens.Add("&&");
                }
                NotTestVisitor newVisitor = new NotTestVisitor(state);
                context.GetChild(i).Accept(newVisitor);
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