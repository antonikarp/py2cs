
using System;
using Antlr4.Runtime.Misc;

public class OutputVisitor : Python3ParserBaseVisitor<Output> {
    public Output output;
    public override Output VisitArith_expr([NotNull] Python3Parser.Arith_exprContext context)
    {
        output = new Output();
        output.arithExpr = context.Accept(new ArithExprVisitor());
        return output;
    }
}