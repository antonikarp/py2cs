using Antlr4.Runtime.Misc;

public class CompIterVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public CompIterVisitor(State _state)
    {
        result = new LineModel();
        state = _state;
    }

    public override LineModel VisitComp_if([NotNull] Python3Parser.Comp_ifContext context)
    {
        // In this case we exclude the nested list comprehensions, so
        // we assume that we have the following children:

        // Child #0: "if"
        // Child #1: test_nocond (which leads to or_test)
        result.tokens.Add(" where ");
        OrTestVisitor newVisitor = new OrTestVisitor(state);
        context.GetChild(1).Accept(newVisitor);
        for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
        {
            result.tokens.Add(newVisitor.result.tokens[i]);
        }
        return result;
    }
    public override LineModel VisitComp_for([NotNull] Python3Parser.Comp_forContext context)
    {
        // For now we assume that the list comprehension is nested one level deep.
        // We have the following children:
        // Child #0: for
        // Child #1: exprlist (for now assume that it is a single expr)
        // Child #2: in
        // Child #3: or_test
        if (context.ChildCount == 4)
        {
            result.tokens.Add(" from ");
            ExprVisitor iteratorVisitor = new ExprVisitor(state);
            context.GetChild(1).Accept(iteratorVisitor);
            for (int i = 0; i < iteratorVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(iteratorVisitor.result.tokens[i]);
            }
            result.tokens.Add(" in ");
            OrTestVisitor collectionVisitor = new OrTestVisitor(state);
            context.GetChild(3).Accept(collectionVisitor);
            for (int i = 0; i < collectionVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(collectionVisitor.result.tokens[i]);
            }
        }
        return result;
    }
}
