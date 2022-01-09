using Antlr4.Runtime.Misc;
public class TestlistStarExprVisitor : Python3ParserBaseVisitor<ExprlistModel>
{
    public ExprlistModel result;
    public State state;
    public TestlistStarExprVisitor(State _state)
    {
        state = _state;
    }
    public override ExprlistModel VisitTestlist_star_expr([NotNull] Python3Parser.Testlist_star_exprContext context)
    {
        result = new ExprlistModel();
        // We assume that we have the following children:
        //
        // Child #0: test
        // (Child #1: ","
        //  Child #2: test
        //  Child #3: ","
        //  ...) - optional
        int i = 0;
        int n = context.ChildCount;
        while (i < n)
        {
            TestVisitor newVisitor = new TestVisitor(state);
            context.GetChild(i).Accept(newVisitor);
            result.expressions.Add(newVisitor.result.ToString());
            i += 2;
        }
        return result;
    }

}
