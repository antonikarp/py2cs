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

        // We can also have a call to the inner constructor:
        // self.B(arg), where B is an inner class.
        if (context.ChildCount == 3 &&
            context.GetChild(0).GetType().ToString() == "Python3Parser+AtomContext" &&
            context.GetChild(1).GetType().ToString() == "Python3Parser+TrailerContext" &&
            context.GetChild(2).GetType().ToString() == "Python3Parser+TrailerContext")
        {
            // Expression is not standalone.
            state.stmtState.isStandalone = false;

            AtomVisitor atomVisitor = new AtomVisitor(state);
            MethodNameTrailerVisitor methodNameTrailerVisitor = new MethodNameTrailerVisitor(state);
            MethodArglistTrailerVisitor methodArglistTrailerVisitor = new MethodArglistTrailerVisitor(state);
            context.GetChild(0).Accept(atomVisitor);
            context.GetChild(1).Accept(methodNameTrailerVisitor);
            context.GetChild(2).Accept(methodArglistTrailerVisitor);
            string varName = atomVisitor.result.ToString();
            

            // Method "append" on a list.
            if (state.output.currentClasses.Peek().currentFunctions.Peek().variables.ContainsKey(varName) &&
                state.output.currentClasses.Peek().currentFunctions.Peek().variables[varName] == VarState.Types.List &&
                methodNameTrailerVisitor.result.ToString() == ".append")
            {
                methodNameTrailerVisitor.result.tokens.Clear();
                methodNameTrailerVisitor.result.tokens.Add(".Add");
            }

            // Case of the inner constructor
            if (varName == "self")
            {
                varName = "";
                result.tokens.Add("new ");
                for (int i = 1; i < methodNameTrailerVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(methodNameTrailerVisitor.result.tokens[i]);
                }
                for (int i = 0; i < methodArglistTrailerVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(methodArglistTrailerVisitor.result.tokens[i]);
                }
            }
            else
            {

                for (int i = 0; i < atomVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(atomVisitor.result.tokens[i]);
                }
                for (int i = 0; i < methodNameTrailerVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(methodNameTrailerVisitor.result.tokens[i]);
                }
                for (int i = 0; i < methodArglistTrailerVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(methodArglistTrailerVisitor.result.tokens[i]);
                }
            }
        }

        else if (context.atom() != null && context.atom().ChildCount == 1 && context.atom().NAME() != null)
        {
            AtomVisitor atomVisitor = new AtomVisitor(state);
            context.atom().Accept(atomVisitor);
            string name = atomVisitor.result.ToString();
            // Function call or field.         
            if (name == "print")
            {
                // In this case (print) the arguments are not changing, we can use the standard
                // TrailerVisitor for arguments. See one of the cases in this file.
                name = "Console.WriteLine";
            }
            else if (name == "range")
            {
                if (context.trailer() != null)
                {
                    // Here there are few cases so use a custom RangeTrailerVisitor.
                    state.output.usingDirs.Add("System.Linq");
                    result.tokens.Add("Enumerable.Range");
                    RangeTrailerVisitor newVisitor = new RangeTrailerVisitor(state);
                    context.GetChild(1).Accept(newVisitor);
                    for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
                    {
                        result.tokens.Add(newVisitor.result.tokens[j]);
                    }
                    // We don't consider the trailer anymore. End here.
                    return result;
                }
            }
            // Field.
            else if (name == "self")
            {
                name = "this";
            }
            result.tokens.Add(name);
        }
        else if (context.atom() != null)
        {
            AtomVisitor atomVisitor = new AtomVisitor(state);
            context.atom().Accept(atomVisitor);
            for (int i = 0; i < atomVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(atomVisitor.result.tokens[i]);
            }

        }

        // Function call (ex. hello())
        // or field (ex. self.name)
        if (context.ChildCount == 2 && context.trailer() != null)
        {
            // Expression is not standalone.
            state.stmtState.isStandalone = false;

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