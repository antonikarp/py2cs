using System;
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
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        if (context.compound_stmt() != null)
        {
            IfStmtVisitor newVisitor = new IfStmtVisitor();
            context.Accept(newVisitor);
        }
        return result;
    }


}