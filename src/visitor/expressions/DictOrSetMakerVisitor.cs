﻿using Antlr4.Runtime.Misc;

// This is a visitor for used for getting individual expressions used
// in initializing a set or a dictionary.
public class DictOrSetMakerVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public DictOrSetMakerVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitDictorsetmaker([NotNull] Python3Parser.DictorsetmakerContext context)
    {
        result = new LineModel();
        int n = context.ChildCount;
        bool isDict = false;
        for (int i = 0; i < n; ++i)
        {
            // We have a dictionary when there is at least one symbol ":"
            if (context.GetChild(i).ToString() == ":")
            {
                isDict = true;
            }
        }
        if (isDict)
        {
            state.classState.usingDirs.Add("System.Collections.Generic");
            result.tokens.Add("new Dictionary<object, object> {");
            int j = 0;
            // We assume that we have the following children:

            // Child 0: key_1
            // Child 1: ":"
            // Child 2: val_1
            // Child 3: ","
            // Child 4: key_2
            // ...

            while (j < n)
            {
                OrTestVisitor keyVisitor = new OrTestVisitor(state);
                context.GetChild(j).Accept(keyVisitor);
                j += 2;
                OrTestVisitor valVisitor = new OrTestVisitor(state);
                context.GetChild(j).Accept(valVisitor);
                j += 2;
                // Add preceding comma to every item except to the first one.
                if (j != 4)
                {
                    result.tokens.Add(", ");
                }
                result.tokens.Add("{" + keyVisitor.result.ToString() + ", " +
                    valVisitor.result.ToString() + "}");
            }
            result.tokens.Add("}");
        }
        return result;
    }

}