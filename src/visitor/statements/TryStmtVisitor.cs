using Antlr4.Runtime.Misc;

// This is a visitor to be used to compute a "try" block.

public class TryStmtVisitor : Python3ParserBaseVisitor<BlockModel>
{
    public BlockModel result;
    public State state;
    public TryStmtVisitor(State _state)
    {
        state = _state;
    }
    public override BlockModel VisitTry_stmt([NotNull] Python3Parser.Try_stmtContext context)
    {
        // We have the following children:
        // Child #0: try
        // Child #1: ":"
        // Child #1: suite
        // ( Child #2: except_clause
        //   Child #3: ":"
        //   Child #4: suite ) - optional
        // ( Child #5: finally
        //   Child #6: ":"
        //   Child #7: suite ) - optional
        result = new BlockModel();

        int n = context.ChildCount;
        int i = 0;

        while (i < n)
        {
            if (context.GetChild(i).ToString() == "try")
            {
                IndentedLine firstLine = new IndentedLine("try", 0);
                result.lines.Add(firstLine);
                IndentedLine openingBraceLine = new IndentedLine("{", 1);
                result.lines.Add(openingBraceLine);
                // A new scope begins.
                state.scopeState = new ScopeState();
                state.scopeState.isActive = true;

                SuiteVisitor suiteVisitor = new SuiteVisitor(state);
                context.GetChild(i + 2).Accept(suiteVisitor);
                int m = suiteVisitor.result.lines.Count;
                for (int j = 0; j < m - 1; ++j)
                {
                    result.lines.Add(suiteVisitor.result.lines[j]);
                }
                // Indent back after the last line.
                IndentedLine lastLine;
                if (m - 1 >= 0)
                {
                    lastLine = new IndentedLine(suiteVisitor.result.lines[m - 1].line, -1);
                }
                else // No lines in the suite.
                {
                    lastLine = new IndentedLine("", -1);
                }
                result.lines.Add(lastLine);
                IndentedLine closingBraceLine = new IndentedLine("}", 0);
                result.lines.Add(closingBraceLine);

                // The scope ends here.
                state.scopeState.isActive = false;
            }
            else if (context.GetChild(i).GetType().ToString() == "Python3Parser+Except_clauseContext")
            {
                ExceptClauseVisitor newVisitor = new ExceptClauseVisitor(state);
                context.GetChild(i).Accept(newVisitor);
                string classNameOfException = newVisitor.result.ToString();
                // catch() -> catch(Exception) : this catches any exception.
                if (classNameOfException == "")
                {
                    classNameOfException = "Exception";
                }
                IndentedLine firstLine = new IndentedLine("catch (" + classNameOfException + ")", 0);
                result.lines.Add(firstLine);
                IndentedLine openingBraceLine = new IndentedLine("{", 1);
                result.lines.Add(openingBraceLine);
                SuiteVisitor suiteVisitor = new SuiteVisitor(state);
                context.GetChild(i + 2).Accept(suiteVisitor);
                int m = suiteVisitor.result.lines.Count;
                for (int j = 0; j < m - 1; ++j)
                {
                    result.lines.Add(suiteVisitor.result.lines[j]);
                }
                // Indent back after the last line.
                IndentedLine lastLine;
                if (m - 1 >= 0)
                {
                    lastLine = new IndentedLine(suiteVisitor.result.lines[m - 1].line, -1);
                }
                else // No lines in the suite.
                {
                    lastLine = new IndentedLine("", -1);
                }
                result.lines.Add(lastLine);
                IndentedLine closingBraceLine = new IndentedLine("}", 0);
                result.lines.Add(closingBraceLine);
            }
            else if (context.GetChild(i).ToString() == "finally")
            {
                IndentedLine firstLine = new IndentedLine("finally", 0);
                result.lines.Add(firstLine);
                IndentedLine openingBraceLine = new IndentedLine("{", 1);
                result.lines.Add(openingBraceLine);
                SuiteVisitor suiteVisitor = new SuiteVisitor(state);
                context.GetChild(i + 2).Accept(suiteVisitor);
                int m = suiteVisitor.result.lines.Count;
                for (int j = 0; j < m - 1; ++j)
                {
                    result.lines.Add(suiteVisitor.result.lines[j]);
                }
                // Indent back after the last line.
                IndentedLine lastLine;
                if (m - 1 >= 0)
                {
                    lastLine = new IndentedLine(suiteVisitor.result.lines[m - 1].line, -1);
                }
                else // No lines in the suite.
                {
                    lastLine = new IndentedLine("", -1);
                }
                result.lines.Add(lastLine);
                IndentedLine closingBraceLine = new IndentedLine("}", 0);
                result.lines.Add(closingBraceLine);

            }
            i += 3;

        }
        return result;
    }

}