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
            // Clear the variable state for a potential new declaration.
            state.varState = new VarState();

            // Initialize stmt state for checking if there is a standalone expression.
            state.stmtState = new StmtState();

            SmallStmtVisitor newVisitor = new SmallStmtVisitor(state);
            // For now we assume that simple_stmt has one child: small_stmt
            context.simple_stmt().Accept(newVisitor);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                sb.Append(newVisitor.result.tokens[i]);
            }
            string line = sb.ToString();


            // Check if we have a standalone expression to be assigned to a
            // discard. We don't completely ignore such an expression because
            // there might be side effects of the used function calls.
            if (state.stmtState.isStandalone)
            {
                IndentedLine lineWithDiscard = new IndentedLine
                    ("_ = " + line + ";", 0);
                result.lines.Add(lineWithDiscard);
            }
            else
            {
                // Add a semicolon at the end of each non-empty line.
                if (line != "")
                {
                    IndentedLine onlyLine = new IndentedLine(line + ";", 0);
                    result.lines.Add(onlyLine);
                }
            }
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