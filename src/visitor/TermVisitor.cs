using Antlr4.Runtime.Misc;

public class TermVisitor : Python3ParserBaseVisitor<Term>
{
    public Term result;
    public override Term VisitTerm([NotNull] Python3Parser.TermContext context)
    {
        result = new Term();
        for (int i = 0; i < context.ChildCount; ++i)
        {
            if (context.GetChild(i).ToString() == "*")
            {
                result.tokens.Add("*");
            }
            else if (context.GetChild(i).ToString() == "/")
            {
                result.tokens.Add("/");
            }
            else
            {
                TermVisitor newVisitor = new TermVisitor();
                context.GetChild(i).Accept(newVisitor);
                for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
                {
                    result.tokens.Add(newVisitor.result.tokens[j]);
                }
            }
        }
        return result;
    }
    public override Term VisitAtom_expr([NotNull] Python3Parser.Atom_exprContext context)
    {
        result = new Term();
        result.tokens.Add(context.atom().NUMBER().ToString());
        return result;
    }
}