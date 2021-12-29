﻿using System.Text;
using System.Collections.Generic;

// This class is a model for a function.
public class Function
{
    public BlockModel statements;
    public string name;
    public bool isVoid;
    public bool isStatic;
    public List<string> parameters;
    public Dictionary<string, string> defaultParameters;
    public Function()
    {
        // By default this value is true, however when the visitor encounters
        // a return statement with expression, it becomes false.
        isVoid = true;
        // For now functions are static, so that they can be called from
        // the static Main method in class Program.
        isStatic = true;
        statements = new BlockModel();
        parameters = new List<string>();
    }
}