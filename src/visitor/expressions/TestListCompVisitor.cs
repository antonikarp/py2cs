using Antlr4.Runtime.Misc;

// This is a visitor for used for getting individual expressions used
// in initializing a list.
// Example:
// "a = [3, 4, 5]" produces 3, 4 and 5
public class TestListCompVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public bool isTuple;
    public int numberOfElements;
    public TestListCompVisitor(State _state)
    {
        state = _state;
        isTuple = false;
        numberOfElements = 0;
    }
    public override LineModel VisitTestlist_comp([NotNull] Python3Parser.Testlist_compContext context)
    {
        result = new LineModel();
        int numberOfElements = 0;
        for (int i = 0; i < context.ChildCount; ++i)
        {
            // Case of lists or tuples. 
            if (context.GetChild(i).ToString() == ",")
            {
                // The type of list would have been assigned already.
                // The other case is Tuple.
                if (state.varState.type != VarState.Types.List &&
                    state.varState.type != VarState.Types.ListFunc)
                {
                    state.varState.type = VarState.Types.Tuple;
                    state.lhsTupleState.isTupleOnLhs = true;

                    // Remember that this is a tuple.
                    isTuple = true;
                }
                result.tokens.Add(", ");
            }
            else
            {
                ++numberOfElements;
                TestVisitor newVisitor = new TestVisitor(state);
                context.GetChild(i).Accept(newVisitor);
                // When declaring a tuple, cast to a nullable type 'int?' when
                // the value is a 'null'.
                // To avoid extremely many names of types in the library function
                // cast it to object.
                // We don't perform any casts when we are on the left-hand side
                // of an assignment.
                if (isTuple && newVisitor.result.ToString() == "null" && !state.lhsState.isLhsState)
                {
                    result.tokens.Add("(object)(int?) null");
                }
                else if (isTuple && !state.lhsState.isLhsState)
                {
                    result.tokens.Add("(object)" + newVisitor.result.ToString());
                }
                else
                {
                    result.tokens.Add(newVisitor.result.ToString());
                }
            }
        }
        if (isTuple && result.tokens.Count >= 1 && !state.lhsState.isLhsState)
        {
            result.tokens[0] = "(object)" + result.tokens[0];
        }
        // If it is a tuple, store the number of elements which will come in
        // handy when computing slices.
        if (isTuple)
        {
            state.tupleState.numberOfElements = numberOfElements;
        }
        return result;
    }

}