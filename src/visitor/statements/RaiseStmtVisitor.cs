using Antlr4.Runtime.Misc;
// This is a visitor for Python "raise" statements used in handling exceptions.
public class RaiseStmtVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public RaiseStmtVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitRaise_stmt([NotNull] Python3Parser.Raise_stmtContext context)
    {
        result = new LineModel();

        if (context.ChildCount == 1)
        {
            // 'raise' without arguments.
            result.tokens.Add("throw");
        }
        else if (context.ChildCount >= 2)
        {
            // For now we assume that we have the following children:
            // Child #0: "raise"
            // Child #1: test -> name of the class
            // (Child #2: from
            // Child #3: test -> name of the previous exception) -- this is for now ignored.

            // raise A -> throw new A();
            // raise A() -> throw new A();
            // new is added when checking for constructor calls.
            result.tokens.Add("throw ");
            TestVisitor nameVisitor = new TestVisitor(state);
            context.GetChild(1).Accept(nameVisitor);
            string name = nameVisitor.result.ToString();

            if ((name.Length < 2) || (name.Length >= 2 && !name.EndsWith(")")))
            {
                result.tokens.Add("new ");
                result.tokens.Add(name);
                result.tokens.Add("()");
            }
            else
            {
                result.tokens.Add(name);
            }
        }
        
        return result;
    }


}