using Antlr4.Runtime.Misc;

// This is a visitor for a term, which can be composed of different
// factors and operators. For now, only the four basic arithmetic expressions are used:
// +, -, *, /
public class TermVisitor : Python3ParserBaseVisitor<Term>
{
    public Term result;

    public void TranslateFloorDivision()
    {
        for (int i = 0; i < result.tokens.Count; ++i)
        {
            if (result.tokens[i] == "//" && i - 1 >= 0 && i + 1 < result.tokens.Count)
            {
                string leftValue = result.tokens[i - 1];
                result.tokens[i - 1] = "";
                string rightValue = result.tokens[i + 1];
                result.tokens[i + 1] = "";
                result.tokens[i] = "Math.Floor(" + leftValue + "/" + rightValue + ")";
            }
        }
    }

    public override Term VisitTerm([NotNull] Python3Parser.TermContext context)
    {
        result = new Term();
        for (int i = 0; i < context.ChildCount; ++i)
        {
            var curChild = context.GetChild(i);
            if (curChild.ToString() == "*")
            {
                result.tokens.Add("*");
            }
            else if (curChild.ToString() == "/")
            {
                result.tokens.Add("/");
            }
            else if (curChild.ToString() == "%")
            {
                result.tokens.Add("%");
            }
            else if (curChild.ToString() == "//")
            {
                result.tokens.Add("//");
            }
            else // We have encountered a factor.
            {
                TermVisitor newVisitor = new TermVisitor();
                // Case of the unary '+' or '-'
                if (curChild.ChildCount == 2)
                {
                    result.tokens.Add(curChild.GetChild(0).ToString());
                    curChild.GetChild(1).Accept(newVisitor);
                }
                // Without unary '+' or '-' - just one child.
                else
                {
                    curChild.Accept(newVisitor);
                }
                for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
                {
                    result.tokens.Add(newVisitor.result.tokens[j]);
                }
            }
        }
        TranslateFloorDivision();
        return result;
    }
    public override Term VisitAtom_expr([NotNull] Python3Parser.Atom_exprContext context)
    {
        result = new Term();
        if (context.atom().ChildCount == 1)
        {
            // Case of numeric literal
            if (context.atom().NUMBER() != null)
            {
                result.tokens.Add(context.atom().NUMBER().ToString());
            }
            else if (context.atom().NAME() != null)
            {
                result.tokens.Add(context.atom().NAME().ToString());
            }
        }
        else if (context.atom().ChildCount == 3 &&
            context.atom().GetChild(0).ToString() == "(" &&
            context.atom().GetChild(2).ToString() == ")")
        {
            result.tokens.Add("(");
            NotTestVisitor internalVisitor = new NotTestVisitor();
            context.Accept(internalVisitor);
            for (int i = 0; i < internalVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(internalVisitor.result.tokens[i]);
            }
            result.tokens.Add(")");
        }
        
        return result;
    }
}