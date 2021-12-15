using System;
using System.Collections.Generic;
public class FuncState
{
    public HashSet<string> declVarNames;
    public bool isVoid;
    public bool isStatic;
    public List<string> parameters;
    public FuncState()
    {
        isVoid = true;
        isStatic = true;
        declVarNames = new HashSet<string>();
        parameters = new List<string>();
    }
}