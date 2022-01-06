using System.Collections.Generic;
public class StmtState
{
    public bool isStandalone;
    // isLocked: used for instance in assignments to force that the expression
    // is not standalone.
    public bool isLocked;
    // isOmitted: for example: assignment to the iterator variable in for loop
    public bool isOmitted;
    public StmtState()
    {
        isOmitted = false; 
        isStandalone = false;
        isLocked = false;
    }
}