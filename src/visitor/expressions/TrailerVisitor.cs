using Antlr4.Runtime.Misc;

public class TrailerVisitor : Python3ParserBaseVisitor<Trailer>
{
    public Trailer result;
    // Right now only 1 argument is handled
    public override Trailer VisitTrailer([NotNull] Python3Parser.TrailerContext context)
    {
        result = new Trailer();
        if (context.ChildCount == 3)
        {
            result.tokens.Add("(");
            OrTestVisitor newVisitor = new OrTestVisitor();
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