using System.Collections.Generic;
public class Library
{
    public Dictionary<string, string> availableClasses;
    public LibConsoleExt libConsoleExt;
    public LibOnceEnumerable libOnceEnumerable;
    public LibIsOperator libIsOperator;
    public LibModuloOperator libModuloOperator;
    public LibDivideByZero libDivideByZero;
    public Output output;
    public HashSet<string> toCommit;
    public Library(Output _output)
    {
        output = _output;
        availableClasses = new Dictionary<string, string>();
        toCommit = new HashSet<string>();

        // Save the code of the classes to availableClasses
        libConsoleExt = new LibConsoleExt(this);
        libOnceEnumerable = new LibOnceEnumerable(this);
        libIsOperator = new LibIsOperator(this);
        libModuloOperator = new LibModuloOperator(this);
        libDivideByZero = new LibDivideByZero(this);
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
    public void CommitIsOperator()
    {
        toCommit.Add(availableClasses["IsOperator"]);
    }
    public void CommitModuloOperator()
    {
        toCommit.Add(availableClasses["ModuloOperator"]);
    }
    public void CommitDivideByZero()
    {
        toCommit.Add(availableClasses["DivideByZero"]);
    }
}