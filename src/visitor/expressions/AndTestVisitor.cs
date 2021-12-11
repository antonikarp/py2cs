using System;
using Antlr4.Runtime.Misc;

// This is a visitor used to compute an expression created with a logical operator "and".
// It traverses the parse tree from the node "and_test".

public class AndTestVisitor : Python3ParserBaseVisitor<AndTest>
{
    public AndTest result;
    public ClassState classState;
    public AndTestVisitor(ClassState _classState)
    {
        classState = _classState;
    }
    public override AndTest VisitAnd_test([NotNull] Python3Parser.And_testContext context)
    {
        result = new AndTest();
        // If there is one child then it is a 'not_test' node.
        if (context.ChildCount == 1)
        {
            NotTestVisitor newVisitor = new NotTestVisitor(classState);
            context.GetChild(0).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        // If there are 3 children then we have an expression:
        // "<expr1> and <expr2>" where <expr1> is the first child and
        // <expr2> is the third child.
        else if (context.ChildCount == 3)
        {
            NotTestVisitor leftVisitor = new NotTestVisitor(classState);
            NotTestVisitor rightVisitor = new NotTestVisitor(classState);
            context.GetChild(0).Accept(leftVisitor);
            context.GetChild(2).Accept(rightVisitor);
            for (int i = 0; i < leftVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(leftVisitor.result.tokens[i]);
            }
            result.tokens.Add("&&");
            for (int i = 0; i < rightVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(rightVisitor.result.tokens[i]);
            }
        }
        return result;
    }



}