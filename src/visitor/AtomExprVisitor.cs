using System;
using System.Text;
public class AtomExprVisitor : Python3ParserBaseVisitor<AtomExpr> {

    public AtomExpr atomExpr; 
    public override AtomExpr VisitAtom_expr(Python3Parser.Atom_exprContext context) {
        atomExpr = new AtomExpr();
        if (context.atom().NAME() != null) {
            if (context.atom().NAME().ToString() == "print") {
                atomExpr.functionName = "Console.WriteLine";
                if (context.ChildCount > 1) {
                    AtomExprVisitor newVisitor = new AtomExprVisitor();
                    context.GetChild(1).Accept(newVisitor);
                    atomExpr.argument = newVisitor.atomExpr.argument;
                }
            }
            
        }
        if (context.atom().STRING().Length > 0) {
            atomExpr.argument = context.atom().STRING().GetValue(0).ToString();
        }
        return atomExpr;
    }
}