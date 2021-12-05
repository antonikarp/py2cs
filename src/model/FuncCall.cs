// This class is a model for a function call.
// For now, it handles the expression: "print(<expr>)".
using System.Text;
public class FuncCall
{
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