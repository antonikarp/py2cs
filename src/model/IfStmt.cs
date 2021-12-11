using System.Text;
using System.Collections.Generic;

// This class is a model for an if statement
public class IfStmt
{
    public List<IndentedLine> lines;
    public IfStmt()
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