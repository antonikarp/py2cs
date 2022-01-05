using System.Collections.Generic;
using Antlr4.Runtime.Misc;

// This is a visitor used for translating method arglist trailer in method calls. 
public class MethodArglistTrailerVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public List<string> arguments;
    public State state;
    public MethodArglistTrailerVisitor(State _state)
    {
        arguments = new List<string>();
        state = _state;
    }
    public override LineModel VisitTrailer([NotNull] Python3Parser.TrailerContext context)
    {
        result = new LineModel();
        // We have the following children if there are no arguments:
        // Child #0: "("
        // Child #1: ")"
        if (context.ChildCount == 2 && context.GetChild(0).ToString() == "("
            && context.GetChild(1).ToString() == ")")
        {
            result.tokens.Add("()");
            return result;
        }

        // We have the following children if there are arguments:
        // Child #0: "("
        // Child #1: arglist
        // Child #2: ")"
        result.tokens.Add("(");
        MethodArglistVisitor newVisitor = new MethodArglistVisitor(state);
        context.GetChild(1).Accept(newVisitor);
        arguments = newVisitor.arguments;
        for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
        {
            result.tokens.Add(newVisitor.result.tokens[i]);
        }
        result.tokens.Add(")");
        return result;
    }

}