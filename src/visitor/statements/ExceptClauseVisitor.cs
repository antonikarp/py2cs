using Antlr4.Runtime.Misc;

// This is a visitor to be used to compute an except clause in a try block.

public class ExceptClauseVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public ExceptClauseVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitExcept_clause([NotNull] Python3Parser.Except_clauseContext context)
    {
        result = new LineModel();
        if (context.ChildCount >= 2)
        {
            // We have the following children:
            // Child #0: "except"
            // Child #1: test

            // or:
            // Child #0: "except"
            // Child #1: test
            // Child #2: "as"
            // Child #3: <identifier>
            TestVisitor newVisitor = new TestVisitor(state);
            context.GetChild(1).Accept(newVisitor);
            string value = newVisitor.result.ToString();
            // Since the names of the exceptions are different in C# than in Python,
            // we need to translate the names on the case to case basis.
            switch (value)
            {
                case "ZeroDivisionError":
                    result.tokens.Add("DivideByZeroException");
                    break;
                case "IndexError":
                    result.tokens.Add("ArgumentOutOfRangeException");
                    break;
                // No match. Try the unchanged name.
                default:
                    result.tokens.Add(value);
                    break;
            }
            if (context.ChildCount == 4)
            {
                // 'except A as a' -> 'catch(A a)'.
                result.tokens.Add(" ");
                result.tokens.Add(context.NAME().ToString());
            }
        }
        return result;
    }
}