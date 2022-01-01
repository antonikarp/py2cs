using Antlr4.Runtime.Misc;

// This is a visitor used to get subscriptions (single index) as well as slices.
public class Subscript_Visitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public Subscript_Visitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitSubscript_([NotNull] Python3Parser.Subscript_Context context)
    {
        result = new LineModel();
        int n = context.ChildCount;
        for (int i = 0; i < n; ++i)
        {
            if (context.GetChild(i).ToString() == ":")
            {
                result.tokens.Add(":");
            }
            else if (context.GetChild(i).GetType().ToString() == "Python3Parser+TestContext")
            {
                TestVisitor newVisitor = new TestVisitor(state);
                context.GetChild(i).Accept(newVisitor);
                for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
                {
                    result.tokens.Add(newVisitor.result.tokens[j]);
                }
            }
            else if (context.GetChild(i).GetType().ToString() == "Python3Parser+SliceopContext")
            {
                SliceopVisitor newVisitor = new SliceopVisitor(state);
                context.GetChild(i).Accept(newVisitor);
                for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
                {
                    result.tokens.Add(newVisitor.result.tokens[j]);
                }
            }
        }
        return result;
    }

}