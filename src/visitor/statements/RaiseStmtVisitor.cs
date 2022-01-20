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
        // For now we assume that we have the following children:
        // Child #0: "raise"
        // Child #1: test -> name of the class
        if (context.ChildCount == 2)
        {
            // raise A -> throw new A();
            // raise A() -> throw new A();
            result.tokens.Add("throw new ");
            TestVisitor nameVisitor = new TestVisitor(state);
            context.GetChild(1).Accept(nameVisitor);
            string name = nameVisitor.result.ToString();
            result.tokens.Add(name);
            if ((name.Length < 2) || (name.Length >= 2 && !name.EndsWith("()")))
            {
                result.tokens.Add("()");
            }
        }
        return result;
    }


}