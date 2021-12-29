using Antlr4.Runtime.Misc;
public class AtomVisitor : Python3ParserBaseVisitor<TokenModel>
{
    public TokenModel result;
    public State state;
    public AtomVisitor(State _state)
    {
        state = _state;
    }
    public override TokenModel VisitAtom([NotNull] Python3Parser.AtomContext context)
    {
        result = new TokenModel();
        // Case of numeric literal
        if (context.NUMBER() != null)
        {
            result.value = context.NUMBER().ToString();
        }
        // Case of string literal
        else if (context.STRING().Length > 0)
        {
            result.value = context.STRING().GetValue(0).ToString();
        }
        // Function calls
        else if (context.NAME() != null)
        {
            result.value = context.NAME().ToString();
        }
        else if (context.TRUE() != null)
        {
            result.value = "true";
        }
        else if (context.FALSE() != null)
        {
            result.value = "false";
        }
        return result;
    }


}