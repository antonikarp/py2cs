using System;
using System.Collections.Generic;
public class ClassState
{
    public HashSet<string> usingDirs;
    public ClassState()
    {
        usingDirs = new HashSet<string>();
        usingDirs.Add("System");
    }
}
