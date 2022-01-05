public class StmtState
{
    public bool isStandalone;
    // isLocked: used for instance in assignments to force that the expression
    // is not standalone.
    public bool isLocked;
    public StmtState()
    {
        isStandalone = false;
        isLocked = false;
    }
}