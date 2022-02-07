using Antlr4.Runtime.Misc;
// This is a visitor for Python 'global' statement. It adds identifiers to
// static fields, because there is no global variables in C#.
public class GlobalStmtVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public GlobalStmtVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitGlobal_stmt([NotNull] Python3Parser.Global_stmtContext context)
    {
        result = new LineModel();
        // We assume that we have the following children:
        // Child #0: 'global'
        // Child #1: <identifier-1>
        // (Child #2: ','
        //  Child #3: <identifier-2>
        //  ...) - optional
        int n = context.ChildCount;
        int i = 1;
        while (i < n)
        {
            string identifier = context.GetChild(i).ToString();
            // We mark that in this function, this identifier refers to a global variable.
            state.output.currentClasses.Peek().currentFunctions.Peek().identifiersReferringToGlobal.Add(identifier);
            i += 2;
        }
        return result;
    }
}