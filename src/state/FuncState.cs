﻿using System;
using System.Collections.Generic;
public class FuncState
{
    public bool isVoid;
    public bool isStatic;
    public List<string> parameters;
    public Dictionary<string, VarState.Types> variables;
    public FuncState()
    {
        isVoid = true;
        isStatic = true;
        parameters = new List<string>();
        variables = new Dictionary<string, VarState.Types>();
    }
}