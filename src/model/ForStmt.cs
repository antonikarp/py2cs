using System.Text;
using System.Collections.Generic;

// This class is a model for a "for" statement
public class ForStmt
{
    public List<IndentedLine> lines;
    public ForStmt()
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