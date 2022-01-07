using System;
using Antlr4.Runtime.Misc;

// This is a visitor to be used to compute a "for" loop
public class ForStmtVisitor : Python3ParserBaseVisitor<BlockModel>
{
    public BlockModel result;
    public State state;
    public ForStmtVisitor(State _state)
    {
        state = _state;
    }
    public override BlockModel VisitFor_stmt([NotNull] Python3Parser.For_stmtContext context)
    {
        state.forStmtState = new ForStmtState();
        // Store the information whether we have an else block;
        if (context.ELSE() != null)
        {
            state.forStmtState.hasElseBlock = true;
            ++state.output.currentClasses.Peek().currentFunctions.Peek().currentGeneratedElseBlockEntryNumber;
        }

        result = new BlockModel();

        // We assume that we have the following children (if there is no else block).

        // Child 0: "for"
        // Child 1: exprlist
        // Child 2: "in"
        // Child 3: testlist
        // Child 4: ":"
        // Child 5: suite

        // For now, we assume that exprlist and testlist have only one child
        // (single expr and single test).

        // If there is an "else" block, then we have additionaly the following children:
        // Child 6: "else"
        // Child 7: ":"
        // Child 8: suite

        ExprVisitor iteratorVisitor = new ExprVisitor(state);
        context.GetChild(1).Accept(iteratorVisitor);
        TestVisitor collectionVisitor = new TestVisitor(state);
        context.GetChild(3).Accept(collectionVisitor);
        string line = "foreach (dynamic " + iteratorVisitor.result.ToString() + " in " +
            collectionVisitor.result.ToString();

        // Mark the variable as the iteration variable. It cannot be assigned to.
        state.forStmtState.forStmtIterationVariable = iteratorVisitor.result.ToString();

        // Check type of the collection. If it is Dictionary then add the property Keys
        if (state.output.currentClasses.Peek().currentFunctions.Peek().variables.ContainsKey(collectionVisitor.result.ToString()))
        {
            VarState.Types type = state.output.currentClasses.Peek().currentFunctions.Peek().variables[collectionVisitor.result.ToString()];
            if (type == VarState.Types.Dictionary)
            {
                line += ".Keys";
            }
        }
        line += ")";
        
        IndentedLine newLine = new IndentedLine(line, 0);
        result.lines.Add(newLine);
        IndentedLine openingBraceLine = new IndentedLine("{", 1);
        result.lines.Add(openingBraceLine);
        SuiteVisitor suiteVisitor = new SuiteVisitor(state);
        context.GetChild(5).Accept(suiteVisitor);
        int n = suiteVisitor.result.lines.Count;
        for (int j = 0; j < n - 1; ++j)
        {
            result.lines.Add(suiteVisitor.result.lines[j]);
        }
        // Indent back after the last line.
        if (n != 0)
        {
            IndentedLine lastLine = new IndentedLine(suiteVisitor.result.lines[n - 1].line, -1);
            result.lines.Add(lastLine);
        }
        // End the block with a closing brace.
        IndentedLine closingBraceLine = new IndentedLine("}", 0);
        result.lines.Add(closingBraceLine);

        if (state.forStmtState.hasElseBlock)
        {
            SuiteVisitor suiteVisitorElse = new SuiteVisitor(state);
            context.GetChild(8).Accept(suiteVisitorElse);
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
                IndentedLine lastLine = new IndentedLine(suiteVisitorElse.result.lines[m - 1].line, -1);
                result.lines.Add(lastLine);
            }
            // End the block with a closing brace.
            IndentedLine closingBraceLineElse = new IndentedLine("}", 0);
            result.lines.Add(closingBraceLineElse);
        }
        // Flush ForStmtState
        state.forStmtState = new ForStmtState();
        
        return result;
    }
}