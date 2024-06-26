﻿using Antlr4.Runtime.Misc;

// This is a visitor used to process an individual import (possibly with an alias) or a series of imports.
public class DottedNameVisitor : Python3ParserBaseVisitor<TokenModel>
{
    public TokenModel result;
    public State state;
    public DottedNameVisitor(State _state)
    {
        state = _state;
    }
    public override TokenModel VisitDotted_name([NotNull] Python3Parser.Dotted_nameContext context)
    {
        result = new TokenModel();
        // Some children might be dots, for instance: "dir.module"
        for (int i = 0; i < context.ChildCount; ++i)
        {
            result.value += context.GetChild(i).ToString();
        }
        return result;
    }

}