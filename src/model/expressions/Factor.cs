using System.Text;
using System.Collections.Generic;

// This class is a model for a factor.
public class Factor
{
    public List<string> tokens;
    public Factor()
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