﻿using System;
using Antlr4.Runtime.Misc;

// This is a visitor used to compute a yield expression.
public class YieldExprVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public YieldExprVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitYield_expr([NotNull] Python3Parser.Yield_exprContext context)
    {
        result = new LineModel();

        if (context.ChildCount == 2)
        {
            // We have the following children:
            // Child #0: yield
            // Child #1: <test>

            state.funcState.isEnumerable = true;
            state.funcState.isVoid = false;

            // We use IEnumerable from System.Collections.Generic
            state.classState.usingDirs.Add("System.Collections.Generic");

            result.tokens.Add("yield return ");
            TestVisitor newVisitor = new TestVisitor(state);
            context.GetChild(1).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }

        return result;
    }

}