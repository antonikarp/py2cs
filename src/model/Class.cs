using System.Text;
using System.Collections.Generic;
// This class is a model for a class.
public class Class
{
    public string name;
    public Function mainMethod;
    public Stack<Function> currentFunctions;
    public List<Function> functions;
    public Output output;
    public Class(Output _output)
    {
        output = _output;
        mainMethod = new Function(output);
        currentFunctions = new Stack<Function>();
        functions = new List<Function>();
    }
    public void CommitToOutput()
    {
        string firstLine = "class ";
        firstLine += name;
        output.outputBuilder.commitIndentedLine(new IndentedLine(firstLine, 0));
        output.outputBuilder.commitIndentedLine(new IndentedLine("{", 1));
        foreach (var func in functions)
        {
            func.CommitToOutput();
        }

        // Main function is left on the stack.
        //var mainFunction = currentFunctions.Peek();
        //mainFunction.CommitToOutput();

        output.outputBuilder.commitIndentedLine(new IndentedLine("", -1));
        output.outputBuilder.commitIndentedLine(new IndentedLine("}", 0));

    }

}