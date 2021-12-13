using Antlr4.Runtime.Misc;

// This is a visitor for used for getting individual expressions used
// in initializing a list.
// Example:
// "a = [3, 4, 5]" produces 3, 4 and 5
public class TestListCompVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public TestListCompVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitTestlist_comp([NotNull] Python3Parser.Testlist_compContext context)
    {
        result = new LineModel();
        for (int i = 0; i < context.ChildCount; ++i)
        {
            if (context.GetChild(i).ToString() == ",")
            {
                result.tokens.Add(", ");
            }
            else
            {
                OrTestVisitor newVisitor = new OrTestVisitor(state);
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