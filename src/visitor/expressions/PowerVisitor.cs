using Antlr4.Runtime.Misc;

public class PowerVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public PowerVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitPower([NotNull] Python3Parser.PowerContext context)
    {
        result = new LineModel();
        if (context.ChildCount == 1)
        {
            AtomExprVisitor newVisitor = new AtomExprVisitor(state);
            context.GetChild(0).Accept(newVisitor);
            for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
            {
                result.tokens.Add(newVisitor.result.tokens[j]);
            }
        }
        // Power operator
        else if (context.ChildCount == 3)
        {
            // Expression is standalone:
            if (!state.stmtState.isLocked)
            {
                state.stmtState.isStandalone = true;
                state.stmtState.isLocked = true;
            }
            AtomExprVisitor leftVisitor = new AtomExprVisitor(state);
            FactorVisitor rightVisitor = new FactorVisitor(state);
            context.GetChild(0).Accept(leftVisitor);
            context.GetChild(2).Accept(rightVisitor);
            result.tokens.Add("Math.Pow(");
            for (int i = 0; i < leftVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(leftVisitor.result.tokens[i]);
            }
            result.tokens.Add(", ");
            for (int i = 0; i < rightVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(rightVisitor.result.tokens[i]);
            }
            result.tokens.Add(")");
        }
        return result;
    }

}