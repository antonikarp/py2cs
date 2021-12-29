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
            // Case of lists or tuples. 
            if (context.GetChild(i).ToString() == ",")
            {
                // The type of list would have been assigned already.
                // The other case is Tuple.
                if (state.varState.type != VarState.Types.List)
                {
                    state.varState.type = VarState.Types.Tuple;
                }
                result.tokens.Add(", ");
            }
            else
            {
                TestVisitor newVisitor = new TestVisitor(state);
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