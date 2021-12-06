using System.Text;

// This class is a model for variable declarations.
// For now, only the most basic way of introducing new variables is used:
// through an expression "a = <expr>", where 'a' is the new variable name.
public class VariableDecl
{
    public string name;
    public NotTest value;
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