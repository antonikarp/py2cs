using Antlr4.Runtime.Misc;
public class SliceopVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public SliceopVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitSliceop([NotNull] Python3Parser.SliceopContext context)
    {
        result = new LineModel();
        int n = context.ChildCount;
        // We have the following children:
        // Child #0: ":"
        // (Child #1: test) - optional
        result.tokens.Add(":");
        if (context.ChildCount == 2)
        {
            TestVisitor newVisitor = new TestVisitor(state);
            context.GetChild(1).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        return result;
    }


}