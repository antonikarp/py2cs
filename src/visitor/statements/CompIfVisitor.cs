using Antlr4.Runtime.Misc;

// This is a visitor to be used to obtain the expression in "if" clause of a list
// comprehension.

public class CompIfVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public CompIfVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitComp_if([NotNull] Python3Parser.Comp_ifContext context)
    {
        result = new LineModel();
        // For now we exclude lambdas and nested list comprehensions, so
        // we assume that we have the following children:

        // Child #0: "if"
        // Child #1: test_nocond (which leads to or_test)
        OrTestVisitor newVisitor = new OrTestVisitor(state);
        context.GetChild(1).Accept(newVisitor);
        for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
        {
            result.tokens.Add(newVisitor.result.tokens[i]);
        }
        return base.VisitComp_if(context);
    }

}