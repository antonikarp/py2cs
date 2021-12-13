using System;
using Antlr4.Runtime.Misc;

// This is a visitor used to compute an expression created with a logical operator "or".
// It traverses the parse tree from the node "or_test".

public class OrTestVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public OrTestVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitOr_test([NotNull] Python3Parser.Or_testContext context)
    {
        result = new LineModel();
        // If there is one child then it is a 'and_test' node.
        if (context.ChildCount == 1)
        {
            AndTestVisitor newVisitor = new AndTestVisitor(state);
            context.GetChild(0).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        // If there are 3 children then we have an expression:
        // "<expr1> or <expr2>" where <expr1> is the first child and
        // <expr2> is the third child.
        else if (context.ChildCount == 3)
        {
            AndTestVisitor leftVisitor = new AndTestVisitor(state);
            AndTestVisitor rightVisitor = new AndTestVisitor(state);
            context.GetChild(0).Accept(leftVisitor);
            context.GetChild(2).Accept(rightVisitor);
            for (int i = 0; i < leftVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(leftVisitor.result.tokens[i]);
            }
            result.tokens.Add("||");
            for (int i = 0; i < rightVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(rightVisitor.result.tokens[i]);
            }
        }
        return result;
    }

}