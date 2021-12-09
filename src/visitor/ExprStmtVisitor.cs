using System;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;

public class ExprStmtVisitor : Python3ParserBaseVisitor<ExprStmt>
{
    public ExprStmt result;
    public override ExprStmt VisitExpr_stmt([NotNull] Python3Parser.Expr_stmtContext context)
    {
        result = new ExprStmt();
        if (context.ChildCount == 3 && context.GetChild(1).ToString() == "=")
        {
            VariableDeclVisitor newVisitor = new VariableDeclVisitor();
            context.Accept(newVisitor);
            List<string> tokens = newVisitor.result.getTokens();
            for (int i = 0; i < tokens.Count; ++i)
            {
                result.tokens.Add(tokens[i]);
            }
            return result;

        }
        else if (context.ChildCount == 1)
        {
            OrTestVisitor newVisitor = new OrTestVisitor();
            context.Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        return result;
    }

}