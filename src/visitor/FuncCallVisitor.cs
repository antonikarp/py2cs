using System;
using System.Text;

// This is a visitor to be used to compute an object of type FuncCall, which
// is later translated to a function call in C#. For now, it handles the expression:
// "print(<expr>)".
public class FuncCallVisitor : Python3ParserBaseVisitor<FuncCall>
{
    public FuncCall atomExpr; 
    public override FuncCall VisitAtom_expr(Python3Parser.Atom_exprContext context)
    {
        atomExpr = new FuncCall();
        if (context.atom().NAME() != null)
        {
            if (context.atom().NAME().ToString() == "print")
            {
                atomExpr.functionName = "Console.WriteLine";
                if (context.ChildCount > 1)
                {
                    FuncCallVisitor newVisitor = new FuncCallVisitor();
                    context.GetChild(1).Accept(newVisitor);
                    atomExpr.argument = newVisitor.atomExpr.argument;
                }
            }
            
        }
        if (context.atom().STRING().Length > 0)
        {
            atomExpr.argument = context.atom().STRING().GetValue(0).ToString();
        }
        return atomExpr;
    }
}