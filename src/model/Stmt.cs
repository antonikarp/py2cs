using System.Text;
using System.Collections.Generic;

// This class is a model for a statement
public class Stmt
{
    public List<string> tokens;
    public Stmt()
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