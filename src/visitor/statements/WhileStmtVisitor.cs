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
        state.loopState = new LoopState();
        state.loopState.loopType = LoopState.LoopType.WhileLoop;
        // Store the information whether we have an else block;
        if (context.ELSE() != null)
        {
            state.loopState.hasElseBlock = true;
            ++state.output.currentClasses.Peek().currentFunctions.Peek().currentGeneratedElseBlockEntryNumber;
        }

        result = new BlockModel();
        // We assume that we have the following children:

        // Child 0: "while"
        // Child 1: test
        // Child 2: ":"
        // Child 3: suite

        // If there is an "else" block, then we have additionally the following children:
        // Child 4: "else"
        // Child 5: ":"
        // Child 6: suite
        TestVisitor conditionVisitor = new TestVisitor(state);
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
        IndentedLine lastLine;
        if (n - 1 >= 0)
        {
            lastLine = new IndentedLine(suiteVisitor.result.lines[n - 1].line, -1);    
        }
        else // No lines in the suite.
        {
            lastLine = new IndentedLine("", -1);
        }
        result.lines.Add(lastLine);
        // End the block with a closing brace.
        IndentedLine closingBraceLine = new IndentedLine("}", 0);
        result.lines.Add(closingBraceLine);

        if (state.loopState.hasElseBlock)
        {
            SuiteVisitor suiteVisitorElse = new SuiteVisitor(state);
            context.GetChild(6).Accept(suiteVisitorElse);
            IndentedLine firstLineElse = new IndentedLine("if ("
                + "_generated_else_entry_"
                + state.output.currentClasses.Peek().currentFunctions.Peek().currentGeneratedElseBlockEntryNumber
                + ")", 0);
            result.lines.Add(firstLineElse);
            IndentedLine openingBraceLineElse = new IndentedLine("{", 1);
            result.lines.Add(openingBraceLine);
            int m = suiteVisitorElse.result.lines.Count;
            for (int j = 0; j < m - 1; ++j)
            {
                result.lines.Add(suiteVisitorElse.result.lines[j]);
            }
            // Indent back after the last line.
            if (m != 0)
            {
                IndentedLine lastLineElse = new IndentedLine(suiteVisitorElse.result.lines[m - 1].line, -1);
                result.lines.Add(lastLineElse);
            }
            // End the block with a closing brace.
            IndentedLine closingBraceLineElse = new IndentedLine("}", 0);
            result.lines.Add(closingBraceLineElse);

        }
        // Flush LoopState
        state.loopState = new LoopState();


        return result;
    }
}