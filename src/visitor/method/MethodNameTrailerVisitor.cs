using Antlr4.Runtime.Misc;

// This is a visitor used for translating method names in method calls. 
public class MethodNameTrailerVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public MethodNameTrailerVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitTrailer([NotNull] Python3Parser.TrailerContext context)
    {
        result = new LineModel();
        // We have the following children:
        // Child #0: "."
        // Child #1: name of the method
        result.tokens.Add(".");
        result.tokens.Add(context.NAME().ToString());
        return result;
    }

}