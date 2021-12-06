using System;
using Antlr4.Runtime.Misc;

// This is a visitor used to compute an expression created with a logical operator "not".
// It traverses the parse tree from the node "not_test".
public class NotTestVisitor : Python3ParserBaseVisitor<NotTest>
{
    public NotTest result;
    public override NotTest VisitNot_test([NotNull] Python3Parser.Not_testContext context)
    {
        result = new NotTest();
        // If there is one child then it is a 'comparison' node.
        if (context.ChildCount == 1)
        {
            ComparisonVisitor newVisitor = new ComparisonVisitor();
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
            result.tokens.Add("!");
            NotTestVisitor newVisitor = new NotTestVisitor();
            context.GetChild(1).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        return result;
    }
}