using Antlr4.Runtime.Misc;

// This is a visitor for a term, which can be composed of different
// factors and operators. For now, only the four basic arithmetic expressions are used:
// +, -, *, /
public class TermVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public TermVisitor(State _state)
    {
        state = _state;
    }
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
                // Cast to double to avoid ambiguity between Math.Floor(decimal) and
                // Math.Floor(double).
                result.tokens[i] = "Math.Floor(" + "(double)" + leftValue + "/" + rightValue + ")";
            }
        }
    }

    public override LineModel VisitTerm([NotNull] Python3Parser.TermContext context)
    {
        result = new LineModel();
        // If there is one child then it is a factor.
        if (context.ChildCount == 1)
        {
            FactorVisitor newVisitor = new FactorVisitor(state);
            context.GetChild(0).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        // If there is more than one child then we have the following children:
        // Child #0: factor
        // Child #1: "*" or "/" or "%" or "//"
        // Child #2: factor
        // ...
        else if (context.ChildCount > 1)
        {
            // Expression is standalone:
            if (!state.stmtState.isLocked)
            {
                state.stmtState.isStandalone = true;
                state.stmtState.isLocked = true;
            }
            int n = context.ChildCount;
            FactorVisitor firstVisitor = new FactorVisitor(state);
            context.GetChild(0).Accept(firstVisitor);
            for (int j = 0; j < firstVisitor.result.tokens.Count; ++j)
            {
                result.tokens.Add(firstVisitor.result.tokens[j]);
            }
            int i = 1;
            while (i + 1 < n)
            {
                if (context.GetChild(i).ToString() == "*")
                {
                    result.tokens.Add("*");
                }
                else if (context.GetChild(i).ToString() == "/")
                {
                    result.tokens.Add("/");
                }
                else if (context.GetChild(i).ToString() == "%")
                {
                    result.tokens.Add("%");
                }
                else if (context.GetChild(i).ToString() == "//")
                {
                    result.tokens.Add("//");
                }
                FactorVisitor newVisitor = new FactorVisitor(state);
                context.GetChild(i + 1).Accept(newVisitor);
                for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
                {
                    result.tokens.Add(newVisitor.result.tokens[j]);
                }
                i += 2;
            }
            TranslateFloorDivision();
        }
        
        return result;
    }
    
}