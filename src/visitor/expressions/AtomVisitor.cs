﻿using Antlr4.Runtime.Misc;
public class AtomVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public AtomVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitAtom([NotNull] Python3Parser.AtomContext context)
    {
        result = new LineModel();

        if (context.ChildCount == 1)
        {
            // Case of numeric literal
            if (context.NUMBER() != null)
            {
                result.tokens.Add(context.NUMBER().ToString());
            }
            // Case of string literal
            else if (context.STRING().Length > 0)
            {
                result.tokens.Add(context.STRING().GetValue(0).ToString());
            }
            // Function name
            else if (context.NAME() != null)
            {
                result.tokens.Add(context.NAME().ToString());
            }
            else if (context.TRUE() != null)
            {
                result.tokens.Add("true");
            }
            else if (context.FALSE() != null)
            {
                result.tokens.Add("false");
            }
        }
        // Expression surrounded by parenthesis.
        else if (context.ChildCount == 3 &&
            context.GetChild(0).ToString() == "(" &&
            context.GetChild(2).ToString() == ")")
        {
            result.tokens.Add("(");
            // This case handles also tuples.
            TestListCompVisitor newVisitor = new TestListCompVisitor(state);
            context.GetChild(1).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
            result.tokens.Add(")");

        }
        // List
        else if (context.ChildCount == 3 &&
            context.GetChild(0).ToString() == "[" &&
            context.GetChild(2).ToString() == "]")
        {
            // We use List from System.Collections.Generic
            state.classState.usingDirs.Add("System.Collections.Generic");
            result.tokens.Add("new List<object> {");
            // We assign the type List before visiting the child.
            state.varState.type = VarState.Types.List;
            TestListCompVisitor newVisitor = new TestListCompVisitor(state);
            context.GetChild(1).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
            result.tokens.Add("}");
        }
        // Dictionary or set
        else if (context.ChildCount == 3 &&
            context.GetChild(0).ToString() == "{" &&
            context.GetChild(2).ToString() == "}")
        {
            DictOrSetMakerVisitor newVisitor = new DictOrSetMakerVisitor(state);
            context.GetChild(1).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }

        return result;
    }


}