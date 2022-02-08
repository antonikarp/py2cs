using Antlr4.Runtime.Misc;
using System.Collections.Generic;
// This is a visitor to be used to compute a "try" block.

public class TryStmtVisitor : Python3ParserBaseVisitor<BlockModel>
{
    public BlockModel result;
    public State state;
    public HashSet<string> exceptionTypesSoFar;
    public TryStmtVisitor(State _state)
    {
        state = _state;
        exceptionTypesSoFar = new HashSet<string>();
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
                string exceptClause = newVisitor.result.ToString();
                // Add the except clause only if the type of the exception haven't appeared before in
                // this 'try' block.
                if (!exceptionTypesSoFar.Contains(exceptClause))
                {
                    exceptionTypesSoFar.Add(exceptClause);
                    // Handle multiple types in a tuple:
                    // except (B, C) --> catch (Exception ex) when (ex is B || ex is C)
                    if (exceptClause.Length >= 2 && exceptClause[0] == '(' && exceptClause[exceptClause.Length - 1] == ')')
                    {
                        // Remove not important characters.
                        string newExceptClause = exceptClause.Replace("(", "").Replace(")", "").
                            Replace("object", "").Replace(" ", "");

                        string[] types = newExceptClause.Split(",");
                        newExceptClause = "(Exception ex) when (";
                        for (int j = 0; j < types.Length; ++j)
                        {
                            if (j != 0)
                            {
                                newExceptClause += " || ";
                            }
                            newExceptClause += ("ex is " + types[j]); 
                        }
                        newExceptClause += ")";
                        exceptClause = newExceptClause;
                    }

                    // catch() -> catch(Exception) : this catches any exception.
                    else if (exceptClause == "")
                    {
                        exceptClause = "(Exception)";
                    }
                    else
                    {
                        exceptClause = "(" + exceptClause + ")";
                    }
                    IndentedLine firstLine = new IndentedLine("catch " + exceptClause + "", 0);
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