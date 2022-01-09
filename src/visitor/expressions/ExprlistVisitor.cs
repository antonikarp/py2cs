using Antlr4.Runtime.Misc;

// This is a visitor that is used to obtain the list of expression seperated by commas.
// It is used for example in:
//
// for x, y in tuple_collection
//
// Here the visitor obtains "x" and "y"

public class ExprlistVisitor : Python3ParserBaseVisitor<ExprlistModel>
{
    public ExprlistModel result;
    public State state;
    public ExprlistVisitor(State _state)
    {
        state = _state;
    }
    public override ExprlistModel VisitExprlist([NotNull] Python3Parser.ExprlistContext context)
    {
        result = new ExprlistModel();
        int i = 0;
        int n = context.ChildCount;
        // We assume that we have the following children:
        // Child #0: expr
        // (Child #1: ","
        // Child #2: expr
        // Child #3: ","
        // ...)
        while (i < n)
        {
            ExprVisitor newVisitor = new ExprVisitor(state);
            context.GetChild(i).Accept(newVisitor);
            result.expressions.Add(newVisitor.result.ToString());
            i += 2;
        }
        return result;
    }

}