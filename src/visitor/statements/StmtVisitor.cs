using System;
using System.Text;
using Antlr4.Runtime.Misc;
public class StmtVisitor : Python3ParserBaseVisitor<BlockModel>
{
    public BlockModel result;
    public State state;
    public StmtVisitor(State _state)
    {
        state = _state;
    }
    public override BlockModel VisitStmt([NotNull] Python3Parser.StmtContext context)
    {
        result = new BlockModel();
        if (context.simple_stmt() != null)
        {
            SmallStmtVisitor newVisitor = new SmallStmtVisitor(state);
            // For now we assume that simple_stmt has one child: small_stmt
            context.simple_stmt().Accept(newVisitor);
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
            CompoundStmtVisitor newVisitor = new CompoundStmtVisitor(state);
            context.compound_stmt().Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.lines.Count; ++i)
            {
                result.lines.Add(newVisitor.result.lines[i]);
            }
        }
        return result;
    }


}