using Antlr4.Runtime.Misc;
// This is a visitor for Python 'nonlocal' statement. It stores the 'nonlocal'
// identifiers in the function. They won't be redeclared, so that they will
// refer to the outer scope.
public class NonlocalStmtVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public NonlocalStmtVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitNonlocal_stmt([NotNull] Python3Parser.Nonlocal_stmtContext context)
    {
        result = new LineModel();
        // We assume that we have the following children:
        // Child #0: 'nonlocal'
        // Child #1: <identifier-1>
        // (Child #2: ','
        //  Child #3: <identifier-2>
        //  ...) - optional
        int n = context.ChildCount;
        int i = 1;
        while (i < n)
        {
            string identifier = context.GetChild(i).ToString();
            state.output.currentClasses.Peek().currentFunctions.Peek().identifiersReferringToNonlocal.Add(identifier);
            i += 1;
        }
        return result;
    }

}