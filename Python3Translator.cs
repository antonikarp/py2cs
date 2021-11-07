using System;
using System.Text;
using Antlr4.Runtime.Misc;

public class Python3Translator : Python3ParserBaseListener {
    public StringBuilder output;
    public Python3Translator() {
        output = new StringBuilder();
    } 

    public override void EnterAtom_expr(Python3Parser.Atom_exprContext context)
    {
        if (context.atom().NAME() != null) {
            if (context.atom().NAME().ToString() == "print") {
                output.Append("Console.WriteLine(");
            }
        }
        if (context.atom().STRING().Length > 0) {
            output.Append(context.atom().STRING().GetValue(0));
        }
    }
    public override void ExitAtom_expr(Python3Parser.Atom_exprContext context)
    {
        if (context.atom().NAME() != null) {
            output.Append(");");
        }
    }

}