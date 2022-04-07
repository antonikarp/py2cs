using System.Collections.Generic;
public class Library
{
    public Dictionary<string, string> availableClasses;
    public LibConsoleExt libConsoleExt;
    public LibOnceEnumerable libOnceEnumerable;
    public LibIsOperator libIsOperator;
    public LibModuloOperator libModuloOperator;
    public LibDivideByZero libDivideByZero;
    public LibListSlice libListSlice;
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
        libListSlice = new LibListSlice(this);

        // By default, include "System";
        output.usingDirsLib.Add("System");
    }
    public void CommitConsoleExt()
    {
        output.usingDirsLib.Add("System.Text");
        output.usingDirsLib.Add("System.Collections.Generic");
        output.usingDirsLib.Add("System.Globalization");
        toCommit.Add(availableClasses["ConsoleExt"]);
    }
    public void CommitOnceEnumerable()
    {
        output.usingDirsLib.Add("System.Collections");
        output.usingDirsLib.Add("System.Collections.Generic");
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
    public void CommitListSlice()
    {
        output.usingDirsLib.Add("System.Collections.Generic");
        toCommit.Add(availableClasses["ListSlice"]);
    }
}