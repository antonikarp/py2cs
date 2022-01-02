using System.Text;
using System.Collections.Generic;

// This class is a model for a list of indented lines comprising some part
// of a block of code.
public class BlockModel
{
    public List<IndentedLine> lines;
    public BlockModel()
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