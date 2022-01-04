using Antlr4.Runtime.Misc;
// This is a visitor used to process imports of modules (possibly with aliases).
public class DottedAsNamesVisitor : Python3ParserBaseVisitor<Empty>
{
    public Empty result;
    public State state;
    public DottedAsNamesVisitor(State _state)
    {
        state = _state;
    }
    public override Empty VisitDotted_as_names([NotNull] Python3Parser.Dotted_as_namesContext context)
    {
        result = new Empty();
        // dotted_as_names: dotted_as_name(',' dotted_as_name) *;
        int i = 0;
        int n = context.ChildCount;
        while (i < n)
        {
            DottedAsNameVisitor newVisitor = new DottedAsNameVisitor(state);
            context.GetChild(i).Accept(newVisitor);
            i += 2;
        }
        return result;
    }


}