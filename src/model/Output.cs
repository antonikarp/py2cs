using System.Text;
public class Output {

    public ArithExpr arithExpr;
    public int indentationLevel = 0;
    
    public string getIndentedLine(string str) {
        string result = "";
        for (int i = 0; i < indentationLevel; ++i) {
            result += "\t";
        }
        result += str;
        return result;
    }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(getIndentedLine("using System;"));
        sb.AppendLine(getIndentedLine("class Program"));
        sb.AppendLine(getIndentedLine("{"));
        ++indentationLevel;
        sb.AppendLine(getIndentedLine("static void Main(string[] args)"));
        sb.AppendLine(getIndentedLine("{"));
        ++indentationLevel;
        sb.AppendLine(getIndentedLine(arithExpr.ToString()));
        --indentationLevel;
        sb.AppendLine(getIndentedLine("}"));
        --indentationLevel;
        sb.AppendLine(getIndentedLine("}"));
        return sb.ToString();
    }
}
