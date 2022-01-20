using Antlr4.Runtime.Misc;
// This is a visitor for Python "pass" statement which does nothing - it translates
// to a standalone semicolon.
public class PassStmtVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public PassStmtVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitPass_stmt([NotNull] Python3Parser.Pass_stmtContext context)
    {
        result = new LineModel();
       
        // This is line which cannot be omitted - it is a single semicolon.
        state.stmtState.isPassStmt = true;

        return result;
    }

}