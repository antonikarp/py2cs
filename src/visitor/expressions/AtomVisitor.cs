using Antlr4.Runtime.Misc;
public class AtomVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public AtomVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitAtom([NotNull] Python3Parser.AtomContext context)
    {
        result = new LineModel();
        // Expression is standalone:
        if (!state.stmtState.isLocked)
        {
            state.stmtState.isStandalone = true;
            state.stmtState.isLocked = true;
        }
        if (context.ChildCount == 1)
        {
            // Case of numeric literal
            if (context.NUMBER() != null)
            {
                result.tokens.Add(context.NUMBER().ToString());
            }
            // Case of string literal
            else if (context.STRING().Length > 0)
            {
                result.tokens.Add(context.STRING().GetValue(0).ToString());
            }
            // Function name
            else if (context.NAME() != null)
            {
                result.tokens.Add(context.NAME().ToString());
            }
            else if (context.TRUE() != null)
            {
                result.tokens.Add("true");
            }
            else if (context.FALSE() != null)
            {
                result.tokens.Add("false");
            }
        }
        // Expression surrounded by parenthesis.
        else if (context.ChildCount == 3 &&
            context.GetChild(0).ToString() == "(" &&
            context.GetChild(2).ToString() == ")")
        {
            result.tokens.Add("(");
            // This case handles also tuples.
            TestListCompVisitor newVisitor = new TestListCompVisitor(state);
            context.GetChild(1).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
            result.tokens.Add(")");

        }
        // List
        else if (context.ChildCount == 3 &&
            context.GetChild(0).ToString() == "[" &&
            context.GetChild(2).ToString() == "]")
        {
            // If it exists, get a list comprehension.
            CompForVisitor compForVisitor = new CompForVisitor(state);
            context.GetChild(1).Accept(compForVisitor);

            // We use List from System.Collections.Generic
            state.output.usingDirs.Add("System.Collections.Generic");

            // List comprehension not found.
            if (compForVisitor.visited == false)
            {
                result.tokens.Add("new List<dynamic> {");
                // We assign the type List before visiting the child.
                state.varState.type = VarState.Types.List;
                TestListCompVisitor newVisitor = new TestListCompVisitor(state);
                context.GetChild(1).Accept(newVisitor);
                for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(newVisitor.result.tokens[i]);
                }
                result.tokens.Add("}");
            }
            else
            {
                // We use Linq for list comprehension
                state.output.usingDirs.Add("System.Linq");
                for (int i = 0; i < compForVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(compForVisitor.result.tokens[i]);
                }
                result.tokens.Add(" select ");
                TestVisitor exprVisitor = new TestVisitor(state);
                // Force the collection of tokens only in the leftmost child where
                // the expression in the list comprehension is located.
                context.GetChild(1).GetChild(0).Accept(exprVisitor);
                for (int i = 0; i < exprVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(exprVisitor.result.tokens[i]);
                }
                result.tokens.Add(").ToList()");   
            }
        }
        // Dictionary or set
        else if (context.ChildCount == 3 &&
            context.GetChild(0).ToString() == "{" &&
            context.GetChild(2).ToString() == "}")
        {
            DictOrSetMakerVisitor newVisitor = new DictOrSetMakerVisitor(state);
            context.GetChild(1).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }

        return result;
    }


}