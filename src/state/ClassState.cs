using System;
using System.Collections.Generic;
public class ClassState
{
    public HashSet<string> usingDirs;
    public List<Function> functions;
    public ClassState()
    {
        usingDirs = new HashSet<string>();
        functions = new List<Function>();
        usingDirs.Add("System");
    }
}
