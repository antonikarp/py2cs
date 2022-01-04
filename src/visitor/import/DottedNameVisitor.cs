using Antlr4.Runtime.Misc;

// This is a visitor used to process an individual import (possibly with an alias) or a series of imports.
public class DottedNameVisitor : Python3ParserBaseVisitor<TokenModel>
{
    public TokenModel result;
    public State state;
    public DottedNameVisitor(State _state)
    {
        state = _state;
    }
    public override TokenModel VisitDotted_name([NotNull] Python3Parser.Dotted_nameContext context)
    {
        result = new TokenModel();
        result.value = context.NAME().GetValue(0).ToString();
        return result;
    }

}