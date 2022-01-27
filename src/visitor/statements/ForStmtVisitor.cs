using System;
using System.Text;
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
        
        state.loopState = new LoopState();
        state.loopState.loopType = LoopState.LoopType.ForLoop;
        // Store the information whether we have an else block;
        if (context.ELSE() != null)
        {
            state.loopState.hasElseBlock = true;
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

        // For now, we assume that testlist have only one child (single test).
        // exprlist can have multiple children -> tuple unpacking

        // If there is an "else" block, then we have additionally the following children:
        // Child 6: "else"
        // Child 7: ":"
        // Child 8: suite

        ExprlistVisitor iteratorVisitor = new ExprlistVisitor(state);
        context.GetChild(1).Accept(iteratorVisitor);

        // varState is for the collection variable.
        state.varState = new VarState();
        TestVisitor collectionVisitor = new TestVisitor(state);
        context.GetChild(3).Accept(collectionVisitor);
        string line = "";

        // No tuple unpacking: there is only one expression in the list
        if (iteratorVisitor.result.expressions.Count == 1)
        {
            string collection = collectionVisitor.result.ToString();
            if (state.output.currentClasses.Peek().currentFunctions.Peek().listIdentifiersToClassNames.ContainsKey(collection))
            {
                line += "foreach (" + state.output.currentClasses.Peek().currentFunctions.Peek().listIdentifiersToClassNames[collection] +
                    " " + iteratorVisitor.result.expressions[0].ToString() + " in " + collection;
            }
            else
            {
                line += "foreach (dynamic " + iteratorVisitor.result.expressions[0].ToString() + " in " + collection;
            }
        }
        // Tuple unpacking: declare a generated varibale: "_tuple"
        else
        {
            line += "foreach (dynamic _tuple in " + collectionVisitor.result.ToString();

        }

        // Mark the variable as the iteration variable. It cannot be assigned to.
        if (iteratorVisitor.result.expressions.Count == 1)
        {
            state.loopState.forStmtIterationVariable = iteratorVisitor.result.expressions[0].ToString();
        }
        else
        {
            state.loopState.forStmtIterationVariable = "_tuple";
        }

        // Declare a static field counterpart of the iteration variable.
        // For now, we don't consider tuples.
        string iteratorIdentifier = state.loopState.forStmtIterationVariable;
        if (!state.output.currentClasses.Peek().staticFieldIdentifiers.Contains(iteratorIdentifier))
        {
            StringBuilder fieldDeclLine = new StringBuilder();
            fieldDeclLine.Append("static dynamic ");
            fieldDeclLine.Append(iteratorIdentifier);
            fieldDeclLine.Append(" = null;");
            IndentedLine fieldDeclIndentedLine = new IndentedLine(fieldDeclLine.ToString(), 0);
            state.output.currentClasses.Peek().staticFieldDeclarations.lines.
                Add(fieldDeclIndentedLine);
            state.output.currentClasses.Peek().staticFieldIdentifiers.Add(iteratorIdentifier);
        }

        

        // Check if the expression represents a dictionary:
        if (state.varState.type == VarState.Types.Dictionary)
        {
            line += ".Keys";
        }
        // Check type of the collection. If it is Dictionary then add the property Keys
        else if (state.output.currentClasses.Peek().currentFunctions.Peek().variables.ContainsKey(collectionVisitor.result.ToString()))
        {
            VarState.Types type = state.output.currentClasses.Peek().currentFunctions.Peek().variables[collectionVisitor.result.ToString()];
            if (type == VarState.Types.Dictionary)
            {
                line += ".Keys";
            }
        }
        // Use reflection to iterate over a tuple
        else if (state.varState.type == VarState.Types.Tuple)
        {
            state.output.usingDirs.Add("System.Linq");
            line += ".GetType().GetFields().Select(x => x.GetValue(";
            line += collectionVisitor.result.ToString();
            line += "))";
        }

        line += ")";


        IndentedLine newLine = new IndentedLine(line, 0);
        result.lines.Add(newLine);
        IndentedLine openingBraceLine = new IndentedLine("{", 1);
        result.lines.Add(openingBraceLine);

        // Update the static field counterpart.
        IndentedLine updateStaticFieldLine = new IndentedLine
            (state.output.currentClasses.Peek().name + "." + iteratorIdentifier + " = " +
            iteratorIdentifier + ";", 0);
        result.lines.Add(updateStaticFieldLine);


        // Tuple unpacking - add additional variables
        if (iteratorVisitor.result.expressions.Count > 1)
        {
            for (int j = 0; j < iteratorVisitor.result.expressions.Count; ++j)
            {
                IndentedLine tupleUnpackingDecl = new IndentedLine
                    ("dynamic " + iteratorVisitor.result.expressions[j]
                    + " = _tuple.Item" + (j + 1).ToString() + ";", 0);
                result.lines.Add(tupleUnpackingDecl);
            }
        }

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

        if (state.loopState.hasElseBlock)
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
                IndentedLine lastLineElse = new IndentedLine(suiteVisitorElse.result.lines[m - 1].line, -1);
                result.lines.Add(lastLineElse);
            }
            // End the block with a closing brace.
            IndentedLine closingBraceLineElse = new IndentedLine("}", 0);
            result.lines.Add(closingBraceLineElse);
        }

        // Add identifiers declared inside the loop to the hidden identifiers.
        // They will be useful when checking if a identifier returned by the function is in scope.
        foreach (string identifier in state.loopState.declaredIdentifiers)
        {
            state.output.currentClasses.Peek().currentFunctions.Peek().hiddenIdentifiers.Add(identifier);
        }

        // Flush LoopState
        state.loopState = new LoopState();
        
        return result;
    }
}