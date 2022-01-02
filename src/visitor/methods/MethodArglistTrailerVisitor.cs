using Antlr4.Runtime.Misc;

// This is a visitor used for translating method arglist trailer in method calls. 
public class MethodArglistTrailerVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public MethodArglistTrailerVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitTrailer([NotNull] Python3Parser.TrailerContext context)
    {
        result = new LineModel();
        // We have the following child:
        // Child #0: "("
        // Child #1: arglist
        // Child #2: ")"
        result.tokens.Add("(");
        MethodArglistVisitor newVisitor = new MethodArglistVisitor(state);
        context.GetChild(1).Accept(newVisitor);
        for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
        {
            result.tokens.Add(newVisitor.result.tokens[i]);
        }
        result.tokens.Add(")");
        return result;
    }

}