using System;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using System.Text;

// This is a visitor called on the tip of the entire tree. It obtains the complete
// source code of the translated program using various different visitors.
public class OutputVisitor : Python3ParserBaseVisitor<State> {
    public State state;
    public OutputVisitor(string moduleName)
    {
        state = new State();
        state.output.moduleName = moduleName;
    }
    public override State VisitFile_input([NotNull] Python3Parser.File_inputContext context)
    {
        return VisitChildren(context);
    }
    public override State VisitStmt([NotNull] Python3Parser.StmtContext context)
    {
        // All of these statements belong to the Main function (entry point).
        StmtVisitor newVisitor = new StmtVisitor(state);
        context.Accept(newVisitor);
        for (int i = 0; i < newVisitor.result.lines.Count; ++i)
        {
            state.output.currentClasses.Peek().currentFunctions.Peek().statements.lines.Add(newVisitor.result.lines[i]);
        }
        return state;
    }
}