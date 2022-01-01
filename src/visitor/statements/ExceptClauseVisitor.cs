﻿using Antlr4.Runtime.Misc;

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
        if (context.ChildCount == 2)
        {
            // We have the following children:
            // Child #0: "except"
            // Child #1: test
            TestVisitor newVisitor = new TestVisitor(state);
            context.GetChild(1).Accept(newVisitor);
            string value = newVisitor.result.ToString();
            switch (value)
            {
                case "ZeroDivisionError":
                    result.tokens.Add("DivideByZeroException");
                    break;
                default:
                    result.tokens.Add(value);
                    break;
            }
        }
        return result;
    }
}