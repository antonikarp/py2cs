
using System;
using Antlr4.Runtime.Misc;

// This is a visitor called on the tip of the entire tree. It obtains the complete
// source code of the translated program using various different visitors.
public class OutputVisitor : Python3ParserBaseVisitor<Output> {
    public Output output;
    public override Output VisitExpr_stmt([NotNull] Python3Parser.Expr_stmtContext context)
    {
        output = new Output();
        output.variableDecl = context.Accept(new VariableDeclVisitor());
        return output;
    }
}