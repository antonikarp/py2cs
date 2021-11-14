using System;
using System.Text;
public class CustomVisitor : Python3ParserBaseVisitor<string> {
    public StringBuilder output;
    
    public CustomVisitor() {
        output = new StringBuilder();
    }
    public override string VisitStmt(Python3Parser.StmtContext context) {
        VisitChildren(context);
        output.Append(");");
        return "";
    }
    public override string VisitAtom_expr(Python3Parser.Atom_exprContext context) {
        if (context.atom().NAME() != null) {
            if (context.atom().NAME().ToString() == "print") {
                output.Append("Console.WriteLine(");
            }
        }
        if (context.atom().STRING().Length > 0) {
            output.Append(context.atom().STRING().GetValue(0));
        }
        VisitChildren(context);
        return "";
    }
}