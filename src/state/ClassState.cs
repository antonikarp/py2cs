using System;
using System.Collections.Generic;
public class ClassState
{
    public HashSet<string> usingDirs;
    public List<Function> functions;
    public Stack<Function> currentFunctions;
    public ClassState()
    {
        usingDirs = new HashSet<string>();
        functions = new List<Function>();
        usingDirs.Add("System");
        currentFunctions = new Stack<Function>();
        Function mainFunction = new Function();
        mainFunction.isVoid = true;
        mainFunction.isStatic = true;
        mainFunction.name = "Main";
        currentFunctions.Push(mainFunction);
    }
}
