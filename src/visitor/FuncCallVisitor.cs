using System;
using System.Text;

// This is a visitor to be used to compute an object of type FuncCall, which
// is later translated to a function call in C#. For now, it handles the expression:
// "print(<expr>)".
public class FuncCallVisitor : Python3ParserBaseVisitor<FuncCall>
{
    public FuncCall result; 
    public override FuncCall VisitAtom_expr(Python3Parser.Atom_exprContext context)
    {
        result = new FuncCall();
        if (context.atom().NAME() != null)
        {
            if (context.atom().NAME().ToString() == "print")
            {
                result.functionName = "Console.WriteLine";
                if (context.ChildCount > 1)
                {
                    FuncCallVisitor newVisitor = new FuncCallVisitor();
                    context.GetChild(1).Accept(newVisitor);
                    result.argument = newVisitor.result.argument;
                }
            }
            // We have encountered an expression.
            else
            {
                result.argument = context.atom().NAME().ToString();
            }
            
        }
        // We have encountered a string literal.
        if (context.atom().STRING().Length > 0)
        {
            result.argument = context.atom().STRING().GetValue(0).ToString();
        }
        return result;
    }
}