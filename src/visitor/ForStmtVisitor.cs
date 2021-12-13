using System;
using Antlr4.Runtime.Misc;

// This is a visitor to be used to compute a "for" loop
public class ForStmtVisitor : Python3ParserBaseVisitor<ForStmt>
{
    public ForStmt result;
    public ClassState classState;
    public ForStmtVisitor(ClassState _classState)
    {
        classState = _classState;
    }
    public override ForStmt VisitFor_stmt([NotNull] Python3Parser.For_stmtContext context)
    {
        result = new ForStmt();

        // We assume that we have a following children:

        // Child 0: "for"
        // Child 1: exprlist
        // Child 2: "in"
        // Child 3: testlist
        // Child 4: ":"
        // Child 5: suite

        // For now, we assume that exprlist and testlist have only one child
        // (single expr and single test). We also do not currently consider an
        // "else" block.

        ShiftExprVisitor iteratorVisitor = new ShiftExprVisitor(classState);
        context.GetChild(1).Accept(iteratorVisitor);
        OrTestVisitor collectionVisitor = new OrTestVisitor(classState);
        context.GetChild(3).Accept(collectionVisitor);
        string line = "foreach (dynamic " + iteratorVisitor.result.ToString() + " in " +
            collectionVisitor.result.ToString() + ")";
        IndentedLine newLine = new IndentedLine(line, 0);
        result.lines.Add(newLine);
        IndentedLine openingBraceLine = new IndentedLine("{", 1);
        result.lines.Add(openingBraceLine);
        SuiteVisitor suiteVisitor = new SuiteVisitor(classState);
        context.GetChild(5).Accept(suiteVisitor);
        int n = suiteVisitor.result.lines.Count;
        for (int j = 0; j < n - 1; ++j)
        {
            result.lines.Add(suiteVisitor.result.lines[j]);
        }
        // Indent back after the last line
        IndentedLine lastLine = new IndentedLine(suiteVisitor.result.lines[n - 1].line, -1);
        result.lines.Add(lastLine);
        // End the block with a closing brace.
        IndentedLine closingBraceLine = new IndentedLine("}", 0);
        result.lines.Add(closingBraceLine);
        return result;
    }
}