using Antlr4.Runtime.Misc;

public class FactorVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public FactorVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitFactor([NotNull] Python3Parser.FactorContext context)
    {
        result = new LineModel();

        // Case of the unary '+' or '-' or bitwise '~'
        if (context.ChildCount == 2)
        {
            // Expression is standalone:
            if (!state.stmtState.isLocked)
            {
                state.stmtState.isStandalone = true;
                state.stmtState.isLocked = true;
            }
            
            FactorVisitor newVisitor = new FactorVisitor(state);
            context.GetChild(1).Accept(newVisitor);
            // Merge the unary operator with the first token.
            result.tokens.Add(context.GetChild(0).ToString() + "(" + newVisitor.result.tokens[0]);
            for (int j = 1; j < newVisitor.result.tokens.Count; ++j)
            {
                result.tokens.Add(newVisitor.result.tokens[j]);
            }
            result.tokens.Add(")");
        }

        // One child: 'power'
        else if (context.ChildCount == 1)
        {
            PowerVisitor newVisitor = new PowerVisitor(state);
            context.GetChild(0).Accept(newVisitor);
            for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
            {
                result.tokens.Add(newVisitor.result.tokens[j]);
            }
        }
        return result;

    }

}
