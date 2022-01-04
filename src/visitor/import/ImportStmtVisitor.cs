using Antlr4.Runtime.Misc;
// This is a visitor used to process an import statement
public class ImportStmtVisitor : Python3ParserBaseVisitor<Empty>
{
    public Empty result;
    public State state;
    public ImportStmtVisitor(State _state)
    {
        state = _state;
    }
    
    public override Empty VisitImport_stmt([NotNull] Python3Parser.Import_stmtContext context)
    {
        result = new Empty();
        // import_stmt: import_name | import_from
        // So far, only import_name is supported.
        ImportNameVisitor newVisitor = new ImportNameVisitor(state);
        context.GetChild(0).Accept(newVisitor);
        return result;
    }

}