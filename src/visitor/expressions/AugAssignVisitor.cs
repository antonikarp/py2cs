using Antlr4.Runtime.Misc;
public class AugAssignVisitor : Python3ParserBaseVisitor<TokenModel>
{
    public TokenModel result;
    public State state;
    public AugAssignVisitor(State _state)
    {
        state = _state;
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
            throw new NotImplementedException("Power augmented assignment is not handled.", context.Start.Line);
        }
        else if (context.IDIV_ASSIGN() != null)
        {
            throw new NotImplementedException("Integral division assignment is not handled.", context.Start.Line);
        }
        else if (context.LEFT_SHIFT_ASSIGN() != null)
        {
            result.value = "<<=";
        }
        else if (context.RIGHT_SHIFT_ASSIGN() != null)
        {
            result.value = ">>=";
        }
        else if (context.AND_ASSIGN() != null)
        {
            result.value = "&=";
        }
        else if (context.OR_ASSIGN() != null)
        {
            result.value = "|=";
        }
        else if (context.XOR_ASSIGN() != null)
        {
            result.value = "^=";
        }
        return result;
    }
}