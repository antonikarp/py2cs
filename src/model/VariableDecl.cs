using System.Text;
using System.Collections.Generic;

// This class is a model for variable declarations.
// For now, only the most basic way of introducing new variables is used:
// through an expression "a = <expr>", where 'a' is the new variable name.
public class VariableDecl
{
    public string name;
    public OrTest value;
    public List<string> getTokens()
    {
        List<string> result = new List<string>();
        result.Add("dynamic");
        result.Add(" ");
        result.Add(name);
        result.Add(" = ");
        for (int j = 0; j < value.tokens.Count; ++j)
        {
            result.Add(value.tokens[j]);
        }
        result.Add(";");
        return result;
    }
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("dynamic ");
        sb.Append(name);
        sb.Append(" = ");
        sb.Append(value.ToString());
        sb.Append(";");
        return sb.ToString();
    }
}