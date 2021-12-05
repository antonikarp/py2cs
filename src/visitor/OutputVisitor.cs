using System;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;

// This is a visitor called on the tip of the entire tree. It obtains the complete
// source code of the translated program using various different visitors.
public class OutputVisitor : Python3ParserBaseVisitor<Output> {
    public Output output;
    public override Output VisitFile_input([NotNull] Python3Parser.File_inputContext context)
    {
        output = new Output();
        output.internalLines = new List<string>();
        return VisitChildren(context);
    }
    public override Output VisitExpr_stmt([NotNull] Python3Parser.Expr_stmtContext context)
    {
        if (context.ChildCount == 3)
        {
            VariableDeclVisitor declVisitor = new VariableDeclVisitor();
            declVisitor.Visit(context);
            output.internalLines.Add(declVisitor.result.ToString());
        }
        else if (context.ChildCount == 1)
        {
            FuncCallVisitor callVisitor = new FuncCallVisitor();
            callVisitor.Visit(context);
            output.internalLines.Add(callVisitor.result.ToString());
        }
        return output;
    }
}