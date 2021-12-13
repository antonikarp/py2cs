using System.Text;
using System.Collections.Generic;

// This class is a model for a list of tokens comprising some part of a single
// line of code.
public class LineModel
{
    public List<string> tokens;
    public LineModel()
    {
        tokens = new List<string>();
    }
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < tokens.Count; ++i)
        {
            sb.Append(tokens[i]);
        }
        return sb.ToString();
    }
}