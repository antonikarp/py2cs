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
    public HashSet<string> usingDirs;
    public Output()
    {
        internalLines = new List<IndentedLine>();
        outputBuilder = new OutputBuilder();
        currentClasses = new Stack<Class>();
        usingDirs = new HashSet<string>();
        // Class Program
        Class programClass = new Class(outputBuilder);
        programClass.name = "Program";
        currentClasses.Push(programClass);
        // Add System in using directives.
        usingDirs.Add("System");
    }

    public override string ToString()
    {
        foreach (var dir in usingDirs)
        {
            outputBuilder.commitIndentedLine(new IndentedLine("using " + dir + ";", 0));
        }
        outputBuilder.commitIndentedLine(new IndentedLine("class Program", 0));
        outputBuilder.commitIndentedLine(new IndentedLine("{", 1));

        foreach (var func in currentClasses.Peek().functions)
        {
            func.outputBuilder = outputBuilder;
            func.CommitToOutput();
        }
        // Main function:
        var mainFunction = currentClasses.Peek().currentFunctions.Peek();
        mainFunction.outputBuilder = outputBuilder;
        mainFunction.CommitToOutput();

        outputBuilder.commitIndentedLine(new IndentedLine("", -1));
        outputBuilder.commitIndentedLine(new IndentedLine("}", 0));
        return outputBuilder.output.ToString();
    }
}
