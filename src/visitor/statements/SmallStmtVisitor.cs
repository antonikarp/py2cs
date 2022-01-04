using System;
using System.Text;
using Antlr4.Runtime.Misc;
public class SmallStmtVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public SmallStmtVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitSmall_stmt([NotNull] Python3Parser.Small_stmtContext context)
    {
        result = new LineModel();
        if (context.expr_stmt() != null)
        {
            ExprStmtVisitor newVisitor = new ExprStmtVisitor(state);
            context.expr_stmt().Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        else if (context.flow_stmt() != null)
        {
            FlowStmtVisitor newVisitor = new FlowStmtVisitor(state);
            context.flow_stmt().Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        else if (context.import_stmt() != null)
        {
            ImportStmtVisitor newVisitor = new ImportStmtVisitor(state);
            context.import_stmt().Accept(newVisitor);
        }
        return result;
    }
}