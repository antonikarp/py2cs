using System.Text;
using System.Collections.Generic;

// This class is a model for atomic expression (atom_expr).
public class AtomExpr
{
    public List<string> tokens;
    public AtomExpr()
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