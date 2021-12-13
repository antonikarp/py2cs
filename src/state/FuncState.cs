using System;
using System.Collections.Generic;
public class FuncState
{
    public HashSet<string> declVarNames;
    public FuncState()
    {
        declVarNames = new HashSet<string>();
    }
}