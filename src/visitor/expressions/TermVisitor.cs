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
                FactorVisitor newVisitor = new FactorVisitor();
                curChild.Accept(newVisitor); 
                for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
                {
                    result.tokens.Add(newVisitor.result.tokens[j]);
                }
            }
        }
        TranslateFloorDivision();
        return result;
    }
    
}