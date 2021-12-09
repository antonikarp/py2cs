using System;
using Antlr4.Runtime.Misc;

// This is a visitor to be used to compute an if statement
public class IfStmtVisitor : Python3ParserBaseVisitor<IfStmt>
{
    public IfStmt result;
    public override IfStmt VisitIf_stmt([NotNull] Python3Parser.If_stmtContext context)
    {
        result = new IfStmt();
        OrTestVisitor conditionVisitor = new OrTestVisitor();
        context.GetChild(1).Accept(conditionVisitor);
        string line = "if (" + conditionVisitor.result.ToString() + ")";
        IndentedLine conditionLine = new IndentedLine(line, 0);
        string line2 = "{";
        IndentedLine openingBraceLine = new IndentedLine(line2, 1);

        // if () { }
        if (context.ChildCount == 4)
        {
            SuiteVisitor newVisitor = new SuiteVisitor();
            context.GetChild(3).Accept(newVisitor);
        }
        return result;
    }

}