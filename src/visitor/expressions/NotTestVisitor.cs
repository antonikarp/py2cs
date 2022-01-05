using System;
using Antlr4.Runtime.Misc;

// This is a visitor used to compute an expression created with a logical operator "not".
// It traverses the parse tree from the node "not_test".
public class NotTestVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public NotTestVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitNot_test([NotNull] Python3Parser.Not_testContext context)
    {
        result = new LineModel();
        // If there is one child then it is a 'comparison' node.
        if (context.ChildCount == 1)
        {
            ComparisonVisitor newVisitor = new ComparisonVisitor(state);
            context.GetChild(0).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        // If there are 2 children then we have an expression: "not <expr>", where
        // <expr> is the second child of type not_test
        else if (context.ChildCount == 2)
        {
            // Expression is standalone:
            if (!state.stmtState.isLocked)
            {
                state.stmtState.isStandalone = true;
                state.stmtState.isLocked = true;
            }
            result.tokens.Add("!");
            NotTestVisitor newVisitor = new NotTestVisitor(state);
            context.GetChild(1).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        return result;
    }
}