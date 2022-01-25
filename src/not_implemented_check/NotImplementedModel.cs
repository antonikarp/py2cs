using System.Collections.Generic;
public class NotImplementedModel
{
    public HashSet<string> declaredFunctions;
    public HashSet<string> declaredClasses;
    public NotImplementedModel()
    {
        declaredFunctions = new HashSet<string>();
        declaredClasses = new HashSet<string>();
    }
}