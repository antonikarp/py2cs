using System;
using System.Text;
using Antlr4.Runtime.Misc;
public class CompoundStmtVisitor : Python3ParserBaseVisitor<CompoundStmt>
{
    public CompoundStmt result;
    public State state;
    public CompoundStmtVisitor(State _state)
    {
        state = _state;
    }
    public override CompoundStmt VisitCompound_stmt([NotNull] Python3Parser.Compound_stmtContext context)
    {
        result = new CompoundStmt();
        if (context.if_stmt() != null)
        {
            IfStmtVisitor newVisitor = new IfStmtVisitor(state);
            context.if_stmt().Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.lines.Count; ++i)
            {
                result.lines.Add(newVisitor.result.lines[i]);
            }
        }
        if (context.for_stmt() != null)
        {
            ForStmtVisitor newVisitor = new ForStmtVisitor(state);
            context.for_stmt().Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.lines.Count; ++i)
            {
                result.lines.Add(newVisitor.result.lines[i]);
            }
        }
            
        return result;
    }


}