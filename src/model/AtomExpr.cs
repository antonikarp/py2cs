using System.Text;
public class AtomExpr {
    public string functionName = "";
    public string argument = "";

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(functionName);
        sb.Append("(");
        sb.Append(argument);
        sb.Append(");");
        return sb.ToString();
    }
}