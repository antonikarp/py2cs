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
        else if (context.ChildCount == 3)
        {
            // Expression is standalone:
            if (!state.stmtState.isLocked)
            {
                state.stmtState.isStandalone = true;
                state.stmtState.isLocked = true;
            }
            NotTestVisitor leftVisitor = new NotTestVisitor(state);
            context.GetChild(0).Accept(leftVisitor);
            NotTestVisitor rightVisitor = new NotTestVisitor(state);
            context.GetChild(2).Accept(rightVisitor);
            VarState.Types leftType = ParamTypeDeduction.Deduce(leftVisitor.result.ToString());
            // Left-hand side is either an int or double
            // The expression "x and y" is equivalent to: x != 0 ? y : x
            if (leftType == VarState.Types.Int || leftType == VarState.Types.Double)
            {

                result.tokens.Add(leftVisitor.result.ToString());
                result.tokens.Add(" != 0 ? ");
                result.tokens.Add(rightVisitor.result.ToString());
                result.tokens.Add(" : ");
                result.tokens.Add(leftVisitor.result.ToString());
            }
            // The expression "x and y" is equivalent to: x.Length != 0 ? y : x
            else if (leftType == VarState.Types.String)
            {
                result.tokens.Add(leftVisitor.result.ToString());
                result.tokens.Add(".Length != 0 ? ");
                result.tokens.Add(rightVisitor.result.ToString());
                result.tokens.Add(" : ");
                result.tokens.Add(leftVisitor.result.ToString());
            }
            // Default or expression
            else
            {
                // Todo: Convert.ToBoolean() might be necessary.
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
        }
        else if (context.ChildCount > 3)
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

                // Explicitly convert to boolean.
                result.tokens.Add("Convert.ToBoolean(");

                context.GetChild(i).Accept(newVisitor);
                for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
                {
                    result.tokens.Add(newVisitor.result.tokens[j]);
                }
                result.tokens.Add(")");
                i += 2;
            }
        }
        return result;
    }



}