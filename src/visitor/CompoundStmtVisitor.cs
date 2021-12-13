using System;
using System.Text;
using Antlr4.Runtime.Misc;
public class CompoundStmtVisitor : Python3ParserBaseVisitor<CompoundStmt>
{
    public CompoundStmt result;
    public ClassState classState;
    public CompoundStmtVisitor(ClassState _classState)
    {
        classState = _classState;
    }
    public override CompoundStmt VisitCompound_stmt([NotNull] Python3Parser.Compound_stmtContext context)
    {
        result = new CompoundStmt();
        if (context.if_stmt() != null)
        {
            IfStmtVisitor newVisitor = new IfStmtVisitor(classState);
            context.if_stmt().Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.lines.Count; ++i)
            {
                result.lines.Add(newVisitor.result.lines[i]);
            }
        }
        if (context.for_stmt() != null)
        {
            ForStmtVisitor newVisitor = new ForStmtVisitor(classState);
            context.for_stmt().Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.lines.Count; ++i)
            {
                result.lines.Add(newVisitor.result.lines[i]);
            }
        }
            
        return result;
    }


}