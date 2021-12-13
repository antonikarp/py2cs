using System;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;

public class ExprStmtVisitor : Python3ParserBaseVisitor<ExprStmt>
{
    public ExprStmt result;
    public State state;
    public ExprStmtVisitor(State _state)
    {
        state = _state;
    }
    public override ExprStmt VisitExpr_stmt([NotNull] Python3Parser.Expr_stmtContext context)
    {
        result = new ExprStmt();
        // This case handles variable declaration and initializtion.
        // Todo: handle assignment.
        if (context.ChildCount == 3 && context.GetChild(1).ToString() == "=")
        {
            OrTestVisitor leftVisitor = new OrTestVisitor(state);
            OrTestVisitor rightVisitor = new OrTestVisitor(state);
            context.GetChild(0).Accept(leftVisitor);
            context.GetChild(2).Accept(rightVisitor);
            result.tokens.Add("dynamic ");
            for (int i = 0; i < leftVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(leftVisitor.result.tokens[i]);
            }
            result.tokens.Add(" = ");
            for (int i = 0; i < rightVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(rightVisitor.result.tokens[i]);
            }
            result.tokens.Add(";");
            return result;

        }
        else if (context.ChildCount == 1)
        {
            OrTestVisitor newVisitor = new OrTestVisitor(state);
            context.Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        return result;
    }

}