using Antlr4.Runtime.Misc;
// This is a visitor used to process "import <module>" or
// "import <module> as <alias>" statement
public class ImportNameVisitor : Python3ParserBaseVisitor<Empty>
{
    public Empty result;
    public State state;
    public ImportNameVisitor(State _state)
    {
        state = _state;
    }
    public override Empty VisitImport_name([NotNull] Python3Parser.Import_nameContext context)
    {
        result = new Empty();
        // We have the following children:
        // Child #0: "import"
        // Child #1: dotted_as_names
        DottedAsNamesVisitor newVisitor = new DottedAsNamesVisitor(state);
        context.GetChild(1).Accept(newVisitor);
        return result;
    }

}