using System.Text;
using System.Collections.Generic;

// This class is a model for an expression statement
public class ExprStmt
{
    public List<string> tokens;
    public ExprStmt()
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