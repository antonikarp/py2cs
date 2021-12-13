using System.Text;
using System.Collections.Generic;

// This class is a model for a while loop
public class WhileStmt
{
    public List<IndentedLine> lines;
    public WhileStmt()
    {
        lines = new List<IndentedLine>();
    }
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < lines.Count; ++i)
        {
            sb.Append(lines[i].line);
        }
        return sb.ToString();
    }
}