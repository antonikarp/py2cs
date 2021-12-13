using Antlr4.Runtime.Misc;

// This visitor is used in translating function calls.
public class TrailerVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public TrailerVisitor(State _state)
    {
        state = _state;
    }
    // Right now only 1 argument is handled
    public override LineModel VisitTrailer([NotNull] Python3Parser.TrailerContext context)
    {
        result = new LineModel();
        if (context.ChildCount == 3)
        {
            result.tokens.Add("(");
            OrTestVisitor newVisitor = new OrTestVisitor(state);
            context.GetChild(1).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
            result.tokens.Add(")");
            result.tokens.Add(";");
        }
        return result;
    }

}