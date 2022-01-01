using System;
using System.Text;
using Antlr4.Runtime.Misc;
public class CompoundStmtVisitor : Python3ParserBaseVisitor<BlockModel>
{
    public BlockModel result;
    public State state;
    public CompoundStmtVisitor(State _state)
    {
        state = _state;
    }
    public override BlockModel VisitCompound_stmt([NotNull] Python3Parser.Compound_stmtContext context)
    {
        result = new BlockModel();
        if (context.if_stmt() != null)
        {
            IfStmtVisitor newVisitor = new IfStmtVisitor(state);
            context.if_stmt().Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.lines.Count; ++i)
            {
                result.lines.Add(newVisitor.result.lines[i]);
            }
        }
        else if (context.for_stmt() != null)
        {
            ForStmtVisitor newVisitor = new ForStmtVisitor(state);
            context.for_stmt().Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.lines.Count; ++i)
            {
                result.lines.Add(newVisitor.result.lines[i]);
            }
        }
        else if (context.while_stmt() != null)
        {
            WhileStmtVisitor newVisitor = new WhileStmtVisitor(state);
            context.while_stmt().Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.lines.Count; ++i)
            {
                result.lines.Add(newVisitor.result.lines[i]);
            }
        }
        else if (context.funcdef() != null)
        {
            FuncdefVisitor newVisitor = new FuncdefVisitor(state);
            context.funcdef().Accept(newVisitor);
            // Do not add to result.lines, the statements are in different function.
        }
        else if (context.try_stmt() != null)
        {
            TryStmtVisitor newVisitor = new TryStmtVisitor(state);
            context.try_stmt().Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.lines.Count; ++i)
            {
                result.lines.Add(newVisitor.result.lines[i]);
            }
        }
        return result;
    }


}