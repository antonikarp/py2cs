using System;
using System.Text;
using Antlr4.Runtime.Misc;
public class StmtVisitor : Python3ParserBaseVisitor<Stmt>
{
    public Stmt result;
    public override Stmt VisitStmt([NotNull] Python3Parser.StmtContext context)
    {
        result = new Stmt();
        if (context.simple_stmt() != null)
        {
            ExprStmtVisitor newVisitor = new ExprStmtVisitor();
            context.Accept(newVisitor);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                sb.Append(newVisitor.result.tokens[i]);
            }
            string line = sb.ToString();
            IndentedLine onlyLine = new IndentedLine(line, 0);
            result.lines.Add(onlyLine);
        }
        else if (context.compound_stmt() != null)
        {
            IfStmtVisitor newVisitor = new IfStmtVisitor();
            context.Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.lines.Count; ++i)
            {
                result.lines.Add(newVisitor.result.lines[i]);
            }
        }
        return result;
    }


}