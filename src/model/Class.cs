using System.Text;
using System.Collections.Generic;
// This class is a model for a class.
public class Class
{
    public string name;
    public Function mainMethod;
    public OutputBuilder outputBuilder;
    public Stack<Function> currentFunctions;
    public List<Function> functions;
    public Class(OutputBuilder _outputBuilder)
    {
        mainMethod = new Function();
        outputBuilder = _outputBuilder;
        currentFunctions = new Stack<Function>();
        functions = new List<Function>();
        // Function Main
        Function mainFunction = new Function();
        mainFunction.isVoid = true;
        mainFunction.isStatic = true;
        mainFunction.name = "Main";
        currentFunctions.Push(mainFunction);
    }
    public void CommitToOutput()
    {
        string firstLine = "class ";
        firstLine += name;
        outputBuilder.commitIndentedLine(new IndentedLine(firstLine, 0));
        outputBuilder.commitIndentedLine(new IndentedLine("{", 1));
        foreach (var func in functions)
        {
            func.outputBuilder = outputBuilder;
            func.CommitToOutput();
        }

        // Main function is left on the stack.
        var mainFunction = currentFunctions.Peek();
        mainFunction.outputBuilder = outputBuilder;
        mainFunction.CommitToOutput();

        outputBuilder.commitIndentedLine(new IndentedLine("", -1));
        outputBuilder.commitIndentedLine(new IndentedLine("}", 0));

    }

}