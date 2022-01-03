using System.Text;
using System.Collections.Generic;
// This class is a model for a class.
public class Class
{
    public string name;
    public BlockModel fieldDecl;
    public List<string> fields;
    public Function mainMethod;
    public Stack<Function> currentFunctions;
    public List<Function> functions;
    public List<Class> internalClasses;
    public Output output;
    public Class(Output _output)
    {
        output = _output;
        mainMethod = new Function(output);
        currentFunctions = new Stack<Function>();
        functions = new List<Function>();
        fieldDecl = new BlockModel();
        fields = new List<string>();
        internalClasses = new List<Class>();
    }
    public void CommitToOutput()
    {
        string firstLine = "public class ";
        firstLine += name;
        output.outputBuilder.commitIndentedLine(new IndentedLine(firstLine, 0));
        output.outputBuilder.commitIndentedLine(new IndentedLine("{", 1));
        foreach (var line in fieldDecl.lines)
        {
            output.outputBuilder.commitIndentedLine(line);
        }
        // Print all internal classes.
        foreach (var internalCls in internalClasses)
        {
            internalCls.CommitToOutput();
        }

        foreach (var func in functions)
        {
            func.CommitToOutput();
        }

        output.outputBuilder.commitIndentedLine(new IndentedLine("", -1));
        output.outputBuilder.commitIndentedLine(new IndentedLine("}", 0));

    }

}