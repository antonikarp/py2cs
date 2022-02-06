using System;
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
            // We have encountered illegal keyword documents. Throw an exception.
            if (state.illegalKeywordArgumentsState.isActive)
            {
                throw new IncorrectInputException("Illegal keyword arguments.");
            }
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
        // List comprehension.
        else if (context.comp_for() != null)
        {
            // We assume that we have the following children:
            // Child #0: test
            // Child #1: comp_for
            TestVisitor newVisitor = new TestVisitor(state);
            context.GetChild(0).Accept(newVisitor);
            CompForVisitor compForVisitor = new CompForVisitor(state);
            context.GetChild(1).Accept(compForVisitor);
            for (int i = 0; i < compForVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(compForVisitor.result.tokens[i]);
            }
            // Add "select <(Child #0).ToString()>" at the end of the list comprehension
            result.tokens.Add(" select " + newVisitor.result.ToString() + ")");
        }
        
        return result;
    }
}