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
        result = new BlockModel();

        // We assume that we have the following children:

        // Child 0: "for"
        // Child 1: exprlist
        // Child 2: "in"
        // Child 3: testlist
        // Child 4: ":"
        // Child 5: suite

        // For now, we assume that exprlist and testlist have only one child
        // (single expr and single test). We also do not currently consider an
        // "else" block.

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
        return result;
    }
}