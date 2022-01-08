using System.Text;
public class OutputBuilder
{
    public StringBuilder output;
    public int indentationLevel;
    public OutputBuilder()
    {
        output = new StringBuilder();
        indentationLevel = 0;
    }
    public void commitIndentedLine(IndentedLine indentedLine)
    {
        // The empty line is for explicitly intending forward or backward without
        // adding any text.
        if (indentedLine.line != "")
        {
            string result = "";
            for (int i = 0; i < indentationLevel; ++i)
            {
                result += "    ";
            }
            result += indentedLine.line;
            output.AppendLine(result);
        }
        if (indentedLine.increment == 1)
        {
            ++indentationLevel;
        }
        else if (indentedLine.increment == -1)
        {
            --indentationLevel;
        }
    }
    public void commitRawCodeBlock(string text)
    {
        string[] lines = text.Split("\n");
        foreach (var line in lines)
        {
            output.AppendLine(line);
        }
    }
}