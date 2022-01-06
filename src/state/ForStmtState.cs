public class ForStmtState
{
    // This holds the name of the iteration variable. This variable cannot be assigned to, so
    // every ExprStmt with this variable on the lhs must be ignored.
    public string forStmtIterationVariable;

    // This indicates whether we have an else block.
    public bool hasElseBlock;

    public ForStmtState()
    {
        forStmtIterationVariable = "";
        hasElseBlock = false;
    }

}