
using System;
public class OutputVisitor : Python3ParserBaseVisitor<Output> {
    public Output output;
    public override Output VisitAtom_expr(Python3Parser.Atom_exprContext context) {
        output = new Output();
        output.atomExpr = context.Accept(new AtomExprVisitor());
        return output;
    }
}