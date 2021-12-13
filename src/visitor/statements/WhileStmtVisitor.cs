using System;
using Antlr4.Runtime.Misc;

// This is a visitor to be used to compute a "while" loop
public class WhileStmtVisitor : Python3ParserBaseVisitor<BlockModel>
{
    public BlockModel result;
    public State state;
    public WhileStmtVisitor(State _state)
    {
        state = _state;
    }
    public override BlockModel VisitWhile_stmt([NotNull] Python3Parser.While_stmtContext context)
    {
        result = new BlockModel();
        // We assume that we have the following children:

        // Child 0: "while"
        // Child 1: test
        // Child 2: ":"
        // Child 3: suite

        // We currently do not consider an "else" block.
        OrTestVisitor conditionVisitor = new OrTestVisitor(state);
        context.GetChild(1).Accept(conditionVisitor);
        string line = "while (" + conditionVisitor.result.ToString() + ")";
        IndentedLine newLine = new IndentedLine(line, 0);
        result.lines.Add(newLine);
        IndentedLine openingBraceLine = new IndentedLine("{", 1);
        result.lines.Add(openingBraceLine);
        SuiteVisitor suiteVisitor = new SuiteVisitor(state);
        context.GetChild(3).Accept(suiteVisitor);
        int n = suiteVisitor.result.lines.Count;
        for (int j = 0; j < n - 1; ++j)
        {
            result.lines.Add(suiteVisitor.result.lines[j]);
        }
        // Indent back after the last line.
        IndentedLine lastLine = new IndentedLine(suiteVisitor.result.lines[n - 1].line, -1);
        result.lines.Add(lastLine);
        // End the block with a closing brace.
        IndentedLine closingBraceLine = new IndentedLine("}", 0);
        result.lines.Add(closingBraceLine);
        return result;
    }
}