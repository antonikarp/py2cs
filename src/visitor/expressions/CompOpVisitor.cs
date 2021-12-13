using Antlr4.Runtime.Misc;

public class CompOpVisitor : Python3ParserBaseVisitor<TokenModel>
{
    public TokenModel result;
    public State state;
    public CompOpVisitor(State _state)
    {
        state = _state;
    }
    public override TokenModel VisitComp_op([NotNull] Python3Parser.Comp_opContext context)
    {
        result = new TokenModel();
        result.value = context.GetText();
        return result;
    }
}