using Antlr4.Runtime.Misc;

// This visitor is used in translating arguments of a function call.

public class ArgumentVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public ArgumentVisitor(State _state)
    {
        state = _state;
    }

    public override LineModel VisitArgument([NotNull] Python3Parser.ArgumentContext context)
    {
        result = new LineModel();
        // Positional argument (usual).
        if (context.ChildCount == 1)
        {
            TestVisitor newVisitor = new TestVisitor(state);
            context.GetChild(0).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        // Named argument (aka keyword argument)
        else if (context.ChildCount == 3 && context.GetChild(1).ToString() == "=")
        {
            TestVisitor keyVisitor = new TestVisitor(state);
            TestVisitor valueVisitor = new TestVisitor(state);
            context.GetChild(0).Accept(keyVisitor);
            context.GetChild(2).Accept(valueVisitor);
            for (int i = 0; i < keyVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(keyVisitor.result.tokens[i]);
            }
            result.tokens.Add(": ");
            for (int i = 0; i < valueVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(valueVisitor.result.tokens[i]);
            }
        }
        
        return result;
    }
}