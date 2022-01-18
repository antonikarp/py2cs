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

            // If we are in a loop ('for' or 'while') with an else statement
            // then we need to append a new statement setting the generated
            // bool variable to be false.
            if (line == "break" && state.loopState.hasElseBlock == true)
            {
                IndentedLine lineSettingToFalse = new IndentedLine(
                    "_generated_else_entry_" +
                    state.output.currentClasses.Peek().currentFunctions.Peek().currentGeneratedElseBlockEntryNumber
                    + " = false;", 0);
                result.lines.Add(lineSettingToFalse);
            }

            // Check if we have a standalone expression to be assigned to a
            // discard. We don't completely ignore such an expression because
            // there might be side effects of the used function calls.
            if (state.stmtState.isStandalone)
            {
                IndentedLine lineWithDiscard = new IndentedLine
                    ("_ = " + line + ";", 0);
                result.lines.Add(lineWithDiscard);
            }
            else if (!state.stmtState.isOmitted)
            {
                // Here we add the obtained line.
                // Add a semicolon at the end of each non-empty line.
                // We make exception for "pass" statement
                if (line != "" || state.stmtState.isPassStmt)
                {
                    // Perform splitting by ";" so that one line containing many
                    // statements is split into many lines.
                    string[] lines = line.Split(";");
                    
                    for (int i = 0; i < lines.Length; ++i)
                    {
                        // Remove empty string, if there was a ";" at the end.
                        if (i == lines.Length - 1 && lines[i] == "")
                        {
                            continue;
                        }
                        IndentedLine newLine = new IndentedLine(lines[i] + ";", 0);
                        result.lines.Add(newLine);
                    }
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