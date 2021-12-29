using Antlr4.Runtime.Misc;
public class AtomExprVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public AtomExprVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitAtom_expr([NotNull] Python3Parser.Atom_exprContext context)
    {

        result = new LineModel();

        // Method call
        // We have the following children:
        // Child #0: atom
        // Child #1: trailer
        // Child #2: trailer
        if (context.ChildCount == 3 &&
            context.GetChild(0).GetType().ToString() == "Python3Parser+AtomContext" &&
            context.GetChild(1).GetType().ToString() == "Python3Parser+TrailerContext" &&
            context.GetChild(2).GetType().ToString() == "Python3Parser+TrailerContext")
        {
            AtomExprVisitor atomVisitor = new AtomExprVisitor(state);
        }

        else if (context.atom().ChildCount == 1)
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
                // In this case (print) the arguments are not changing, we can use the standard
                // TrailerVisitor for arguments.
                if (context.atom().NAME().ToString() == "print")
                {
                    result.tokens.Add("Console.WriteLine");
                }
                else if (context.atom().NAME().ToString() == "range")
                {
                    if (context.trailer() != null)
                    {
                        state.classState.usingDirs.Add("System.Linq");
                        result.tokens.Add("Enumerable.Range");
                        RangeTrailerVisitor newVisitor = new RangeTrailerVisitor(state);
                        context.GetChild(1).Accept(newVisitor);
                        for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
                        {
                            result.tokens.Add(newVisitor.result.tokens[j]);
                        }
                        return result;
                    }
                }
                else
                {
                    result.tokens.Add(context.atom().NAME().ToString());
                }
            }
            else if (context.atom().TRUE() != null)
            {
                result.tokens.Add("true");
            }
            else if (context.atom().FALSE() != null)
            {
                result.tokens.Add("false");
            }
        }

        // Expression sorrounded by parenthesis.
        else if (context.atom().ChildCount == 3 &&
            context.atom().GetChild(0).ToString() == "(" &&
            context.atom().GetChild(2).ToString() == ")")
        {
            result.tokens.Add("(");
            // This case handles also tuples.
            TestListCompVisitor newVisitor = new TestListCompVisitor(state);
            context.atom().GetChild(1).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
            result.tokens.Add(")");
        }


        // List
        else if (context.atom().ChildCount == 3 &&
            context.atom().GetChild(0).ToString() == "[" &&
            context.atom().GetChild(2).ToString() == "]")
        {
            // We use List from System.Collections.Generic
            state.classState.usingDirs.Add("System.Collections.Generic");
            result.tokens.Add("new List<object> {");
            // We assign the type List before visiting the child.
            state.varState.type = VarState.Types.List;
            TestListCompVisitor newVisitor = new TestListCompVisitor(state);
            context.atom().GetChild(1).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
            result.tokens.Add("}");
        }
        // Dictionary or set
        else if (context.atom().ChildCount == 3 &&
            context.atom().GetChild(0).ToString() == "{" &&
            context.atom().GetChild(2).ToString() == "}")
        {
            DictOrSetMakerVisitor newVisitor = new DictOrSetMakerVisitor(state);
            context.atom().GetChild(1).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }

        // Function call
        else if (context.ChildCount == 2 && context.trailer() != null)
        {
            TrailerVisitor newVisitor = new TrailerVisitor(state);
            context.GetChild(1).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }

        }

        

        return result;
    }
}