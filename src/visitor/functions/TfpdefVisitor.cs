using System;
using System.Text;
using Antlr4.Runtime.Misc;

// tfpdef = typed function parameter definition
public class TfpdefVisitor : Python3ParserBaseVisitor<TokenModel>
{
    public TokenModel result;
    public State state;

    public TfpdefVisitor(State _state)
    {
        state = _state;
    }
    public override TokenModel VisitTfpdef([NotNull] Python3Parser.TfpdefContext context)
    {
        result = new TokenModel();
        result.value = context.NAME().ToString();
        return result;
    }
}