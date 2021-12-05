using System.Text;
using System.Collections.Generic;

// This class is a model for a term. Terms compose an arithmetic expression.
// Example:
// arith_expr: 2 + 3
// The terms are 2, 3
public class Term
{
    public List<string> tokens;
    public Term()
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