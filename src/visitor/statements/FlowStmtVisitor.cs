using System;
using System.Text;
using Antlr4.Runtime.Misc;
public class FlowStmtVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public FlowStmtVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitFlow_stmt([NotNull] Python3Parser.Flow_stmtContext context)
    {
        result = new LineModel();
        if (context.break_stmt() != null)
        {
            result.tokens.Add("break;");
        }
        else if (context.continue_stmt() != null)
        {
            result.tokens.Add("continue;");
        }
        return result;
    }


}