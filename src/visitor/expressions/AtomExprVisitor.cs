using Antlr4.Runtime.Misc;
public class AtomExprVisitor : Python3ParserBaseVisitor<AtomExpr>
{
    public AtomExpr result;
    public override AtomExpr VisitAtom_expr([NotNull] Python3Parser.Atom_exprContext context)
    {
        result = new AtomExpr();
        if (context.atom().ChildCount == 1)
        {
            // Case of numeric literal
            if (context.atom().NUMBER() != null)
            {
                result.tokens.Add(context.atom().NUMBER().ToString());
            }
            if (context.atom().STRING().Length > 0)
            {
                result.tokens.Add(context.atom().STRING().GetValue(0).ToString());
            }
            else if (context.atom().NAME() != null)
            {
                if (context.atom().NAME().ToString() == "print")
                {
                    result.tokens.Add("Console.WriteLine");
                }
                else
                {
                    result.tokens.Add(context.atom().NAME().ToString());
                }
            }
        }

        // Expression sorrounded by paranthesis.
        else if (context.atom().ChildCount == 3 &&
            context.atom().GetChild(0).ToString() == "(" &&
            context.atom().GetChild(2).ToString() == ")")
        {
            result.tokens.Add("(");
            OrTestVisitor internalVisitor = new OrTestVisitor();
            context.Accept(internalVisitor);
            for (int i = 0; i < internalVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(internalVisitor.result.tokens[i]);
            }
            result.tokens.Add(")");
        }

        // Function call
        if (context.ChildCount == 2 && context.trailer() != null)
        {
            TrailerVisitor newVisitor = new TrailerVisitor();
            context.GetChild(1).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }

        }
        return result;
    }
}