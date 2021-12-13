using System.Text;
using System.Collections.Generic;

// This class is a model for a compound statement
public class CompoundStmt
{
    public List<IndentedLine> lines;
    public CompoundStmt()
    {
        lines = new List<IndentedLine>();
    }
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < lines.Count; ++i)
        {
            sb.Append(lines[i]);
        }
        return sb.ToString();
    }
}