using System.Text;
using System.Collections.Generic;

// This class is a model for the entire program.
// For now, it translates expressions to the Main method in the Program class.
public class Output
{
    public List<string> internalLines;
    public int indentationLevel = 0;
    
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
        sb.AppendLine(getIndentedLine("using System;"));
        sb.AppendLine(getIndentedLine("class Program"));
        sb.AppendLine(getIndentedLine("{"));
        ++indentationLevel;
        sb.AppendLine(getIndentedLine("static void Main(string[] args)"));
        sb.AppendLine(getIndentedLine("{"));
        ++indentationLevel;
        foreach (var line in internalLines)
        {
            sb.AppendLine(getIndentedLine(line));
        }
        --indentationLevel;
        sb.AppendLine(getIndentedLine("}"));
        --indentationLevel;
        sb.AppendLine(getIndentedLine("}"));
        return sb.ToString();
    }
}
