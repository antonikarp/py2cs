using System;
using System.Collections.Generic;
public class FuncState
{
    public HashSet<string> declVarNames;
    public bool isVoid;
    public FuncState()
    {
        isVoid = true;
        declVarNames = new HashSet<string>();
    }
}