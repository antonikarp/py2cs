using System.Text;
using System.Collections.Generic;

// This class is a model for the entire program.
// For now, it translates expressions to the Main method in the Program class.
public class Output
{
    public List<IndentedLine> internalLines;
    public int indentationLevel = 0;
    public OutputBuilder outputBuilder;
    public Stack<Class> currentClasses;
    public List<Class> classes;
    // allClasses is for checking if we have a constructor call (we need to
    // take into account also the nested classes).
    public List<Class> allClasses;
    public HashSet<string> usingDirs;
    public Output()
    {
        internalLines = new List<IndentedLine>();
        outputBuilder = new OutputBuilder();
        currentClasses = new Stack<Class>();
        usingDirs = new HashSet<string>();
        classes = new List<Class>();
        allClasses = new List<Class>();
        // Class Program.
        Class programClass = new Class(this);
        programClass.name = "Program";
        
        // Function Main - entry point
        Function mainFunction = new Function(this);
        mainFunction.isVoid = true;
        mainFunction.isStatic = true;
        mainFunction.name = "Main";
        mainFunction.parentClass = programClass;
        programClass.currentFunctions.Push(mainFunction);
        programClass.functions.Add(mainFunction);

        currentClasses.Push(programClass);
        classes.Add(programClass);
        allClasses.Add(programClass);

        // Add System in using directives.
        usingDirs.Add("System");
    }

    public override string ToString()
    {
        foreach (var dir in usingDirs)
        {
            outputBuilder.commitIndentedLine(new IndentedLine("using " + dir + ";", 0));
        }
        foreach (var cls in classes)
        {
            cls.CommitToOutput();
        }
        return outputBuilder.output.ToString();
    }
}
