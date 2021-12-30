using Antlr4.Runtime.Misc;

// This visitor is used in translating:
// * function calls
// * subscriptions (ex. dict_a["key"])

public class TrailerVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public TrailerVisitor(State _state)
    {
        state = _state;
    }

    public override LineModel VisitTrailer([NotNull] Python3Parser.TrailerContext context)
    {
        result = new LineModel();
        // Function call - some parameters
        if (context.ChildCount == 3 && context.GetChild(0).ToString() == "(" &&
            context.GetChild(2).ToString() == ")" &&
            context.GetChild(1).GetType().ToString() == "Python3Parser+ArglistContext")
        {
            result.tokens.Add("(");
            // We assume that we have the following children:
            // Child #0: argument_0
            // Child #1: ","
            // Child #2: argument_1
            // ...
            int n = context.GetChild(1).ChildCount;
            int i = 0;
            while (i < n)
            {
                if (i != 0)
                {
                    result.tokens.Add(", ");
                }
                ArgumentVisitor newVisitor = new ArgumentVisitor(state);
                context.GetChild(1).GetChild(i).Accept(newVisitor);
                for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
                {
                    result.tokens.Add(newVisitor.result.tokens[j]);
                }
                i += 2;
            }
            result.tokens.Add(")");
        }
        // Subscription
        else if (context.ChildCount == 3 && context.GetChild(0).ToString() == "[" &&
            context.GetChild(2).ToString() == "]")
        {
            result.tokens.Add("[");
            // Right now only 1 argument is handled.
            TestVisitor newVisitor = new TestVisitor(state);
            context.GetChild(1).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
            result.tokens.Add("]");
        }
        // Function call - no parameters
        else if (context.ChildCount == 2 && context.GetChild(0).ToString() == "(" &&
            context.GetChild(1).ToString() == ")")
        {
            result.tokens.Add("()");
        }
        return result;
    }

}