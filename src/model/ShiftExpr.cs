using System.Text;
using System.Collections.Generic;

// This class is a model for a shift expression (constructed using "<<"
// and ">>" operators).
public class ShiftExpr
{
    public List<string> tokens;
    public ShiftExpr()
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