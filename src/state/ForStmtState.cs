public class ForStmtState
{
    // This holds the name of the iteration variable. This variable cannot be assigned to, so
    // every ExprStmt with this variable on the lhs must be ignored.
    public string forStmtIterationVariable;
}