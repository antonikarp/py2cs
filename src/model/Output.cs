using System.Text;
using System.Collections.Generic;

// This class is a model for the entire program.
// For now, it translates expressions to the Main method in the Program class.
public class Output
{
    public State state;
    public List<IndentedLine> internalLines;
    public int indentationLevel = 0;
    public OutputBuilder outputBuilder;
    public Output()
    {
        state = new State();
        internalLines = new List<IndentedLine>();
        outputBuilder = new OutputBuilder();
    }

    public override string ToString()
    {
        foreach (var dir in state.classState.usingDirs)
        {
            outputBuilder.commitIndentedLine(new IndentedLine("using " + dir + ";", 0));
        }
        outputBuilder.commitIndentedLine(new IndentedLine("class Program", 0));
        outputBuilder.commitIndentedLine(new IndentedLine("{", 1));

        foreach (var func in state.classState.functions)
        {
            func.outputBuilder = outputBuilder;
            func.CommitToOutput();
        }
        // Main function:
        var mainFunction = state.classState.currentFunctions.Peek();
        mainFunction.outputBuilder = outputBuilder;
        mainFunction.CommitToOutput();

        outputBuilder.commitIndentedLine(new IndentedLine("", -1));
        outputBuilder.commitIndentedLine(new IndentedLine("}", 0));
        return outputBuilder.output.ToString();
    }
}
