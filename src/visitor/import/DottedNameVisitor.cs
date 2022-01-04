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

/*public class DottedAsNameVisitor : Python3ParserBaseVisitor<Empty>
{
    public Empty result;
    public State state;
    public DottedAsNameVisitor(State _state)
    {
        state = _state;
    }
    public override Empty VisitDotted_as_name([NotNull] Python3Parser.Dotted_as_nameContext context)
    {
        result = new Empty();
        // So far, we process an individual import without any alias.
        if (context.dotted_name() != null)
        {
            string name = context.dotted_name().NAME().ToString();

        }
        return result;
    }


}*/