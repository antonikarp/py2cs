using System;
using Antlr4.Runtime.Misc;

// This is a visitor used to compute an expression used in assignment.

public class TestVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public TestVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitTest([NotNull] Python3Parser.TestContext context)
    {
        result = new LineModel();
        // Case of a ternary operator
        // We have the following children:

        // Child 0 : or_test
        // Child 1 : "if"
        // Child 2 : or_test
        // Child 3 : "else"
        // Child 4 : test

        if (context.ChildCount == 5)
        {
            OrTestVisitor trueValueVisitor = new OrTestVisitor(state);
            context.GetChild(0).Accept(trueValueVisitor);
            TestVisitor falseValueVisitor = new TestVisitor(state);
            context.GetChild(4).Accept(falseValueVisitor);
            OrTestVisitor conditionVisitor = new OrTestVisitor(state);
            context.GetChild(2).Accept(conditionVisitor);

            // Explicitly convert to boolean.
            result.tokens.Add("Convert.ToBoolean(");

            for (int i = 0; i < conditionVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(conditionVisitor.result.tokens[i]);
            }

            result.tokens.Add(")");

            result.tokens.Add(" ? ");
            for (int i = 0; i < trueValueVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(trueValueVisitor.result.tokens[i]);
            }
            result.tokens.Add(" : ");
            result.tokens.Add("(");
            for (int i = 0; i < falseValueVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(falseValueVisitor.result.tokens[i]);
            }
            result.tokens.Add(")");
        }
        // One child which is or_test.
        else if (context.ChildCount == 1)
        {
            OrTestVisitor newVisitor = new OrTestVisitor(state);
            context.GetChild(0).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        return result;
    }



}