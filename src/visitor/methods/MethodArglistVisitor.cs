using System.Collections.Generic;
using Antlr4.Runtime.Misc;

// This is a visitor used for translating method arglist (arguments) in method calls. 
public class MethodArglistVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public List<string> arguments;
    public State state;
    public MethodArglistVisitor(State _state)
    {
        arguments = new List<string>();
        state = _state;
    }
    public override LineModel VisitArglist([NotNull] Python3Parser.ArglistContext context)
    {
        result = new LineModel();
        // We have the following child:
        // Child #0: argument
        // Child #1: ","
        // Child #2: argument
        // ...
        int n = context.ChildCount;
        int i = 0;
        while (i < n)
        {
            if (i != 0)
            {
                result.tokens.Add(", ");
            }
            ArgumentVisitor newVisitor = new ArgumentVisitor(state);
            context.GetChild(i).Accept(newVisitor);
            for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
            {
                result.tokens.Add(newVisitor.result.tokens[j]);
            }
            arguments.Add(newVisitor.result.ToString());
            i += 2;
        }
        return result;
    }

}