using Antlr4.Runtime.Misc;
public class AugAssignVisitor : Python3ParserBaseVisitor<TokenModel>
{
    public TokenModel result;
    public State state;
    public bool isPowerAssign;
    public AugAssignVisitor(State _state)
    {
        state = _state;
        isPowerAssign = false;
    }
    public override TokenModel VisitAugassign([NotNull] Python3Parser.AugassignContext context)
    {
        result = new TokenModel();
        if (context.ADD_ASSIGN() != null)
        {
            result.value = "+=";
        }
        else if (context.SUB_ASSIGN() != null)
        {
            result.value = "-=";
        }
        else if (context.MULT_ASSIGN() != null)
        {
            result.value = "*=";
        }
        else if (context.DIV_ASSIGN() != null)
        {
            result.value = "/=";
        }
        else if (context.MOD_ASSIGN() != null)
        {
            result.value = "%=";
        }
        else if (context.POWER_ASSIGN() != null)
        {
            isPowerAssign = true;
            result.value = "=";
        }
        return result;
    }
}