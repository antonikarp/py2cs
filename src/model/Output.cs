using System.Text;
using System.Collections.Generic;

// This class is a model for the entire program.
// For now, it translates expressions to the Main method in the Program class.
public class Output
{
    public State state;
    public List<IndentedLine> internalLines;
    public int indentationLevel = 0;

    public Output()
    {
        state = new State();
        internalLines = new List<IndentedLine>();
    }

    public string getIndentedLine(string str)
    {
        string result = "";
        for (int i = 0; i < indentationLevel; ++i)
        {
            result += "    ";
        }
        result += str;
        return result;
    }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var dir in state.classState.usingDirs)
        {
            sb.AppendLine(getIndentedLine("using " + dir + ";"));
        }
        sb.AppendLine(getIndentedLine("class Program"));
        sb.AppendLine(getIndentedLine("{"));
        ++indentationLevel;
        sb.AppendLine(getIndentedLine("static void Main(string[] args)"));
        sb.AppendLine(getIndentedLine("{"));
        ++indentationLevel;
        foreach (var indentedLine in internalLines)
        {
            sb.AppendLine(getIndentedLine(indentedLine.line));
            if (indentedLine.increment == 1)
            {
                ++indentationLevel;
            }
            else if (indentedLine.increment == -1)
            {
                --indentationLevel;
            }
        }
        --indentationLevel;
        sb.AppendLine(getIndentedLine("}"));
        --indentationLevel;
        sb.AppendLine(getIndentedLine("}"));
        return sb.ToString();
    }
}
