using System;
using System.Collections.Generic;
public class FuncState
{
    public bool isVoid;
    public bool isStatic;
    public bool isEnumerable;
    public List<string> parameters;
    public Dictionary<string, string> defaultParameters;
    public Dictionary<string, VarState.Types> defaultParameterTypes;
    public Dictionary<string, VarState.Types> variables;  
    public FuncState()
    {
        isVoid = true;
        isStatic = true;
        parameters = new List<string>();
        variables = new Dictionary<string, VarState.Types>();
        defaultParameters = new Dictionary<string, string>();
        defaultParameterTypes = new Dictionary<string, VarState.Types>();
    }
}