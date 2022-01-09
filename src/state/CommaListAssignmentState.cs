using System.Collections.Generic;
public class CommaListAssignmentState
{
    public string tupleIdentifier;
    public List<string> lhsExpressions;
    public bool isActive;
    public CommaListAssignmentState()
    {
        tupleIdentifier = "";
        lhsExpressions = new List<string>();
        isActive = false;
    }
}