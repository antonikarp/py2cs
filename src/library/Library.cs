using System.Collections.Generic;
public class Library
{
    public Dictionary<string, string> availableClasses;
    public LibConsoleExt libConsoleExt;
    public LibOnceEnumerable libOnceEnumerable;
    public Output output;
    public HashSet<string> toCommit;
    public Library(Output _output)
    {
        output = _output;
        availableClasses = new Dictionary<string, string>();
        toCommit = new HashSet<string>();

        // Save the code of the classes to availbleClasses
        libConsoleExt = new LibConsoleExt(this);
        libOnceEnumerable = new LibOnceEnumerable(this);
    }
    public void CommitConsoleExt()
    {
        output.usingDirs.Add("System.Text");
        output.usingDirs.Add("System.Collections.Generic");
        toCommit.Add(availableClasses["ConsoleExt"]);
    }
    public void CommitOnceEnumerable()
    {
        output.usingDirs.Add("System.Collections");
        output.usingDirs.Add("System.Collections.Generic");
        toCommit.Add(availableClasses["OnceEnumerable"]);
    }
    
}