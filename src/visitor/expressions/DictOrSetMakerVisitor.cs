using Antlr4.Runtime.Misc;

// This is a visitor for used for getting individual expressions used
// in initializing a set or a dictionary.
public class DictOrSetMakerVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public bool isZeroPresent;
    public bool isOnePresent;
    public DictOrSetMakerVisitor(State _state)
    {
        state = _state;
        isZeroPresent = false;
        isOnePresent = false;
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
        // Case of a dictionary.
        if (isDict)
        {
            state.output.usingDirs.Add("System.Collections.Generic");

            // This is a dict comprehension.
            if (context.ChildCount == 4 && context.GetChild(3).GetType().ToString() ==
                "Python3Parser+Comp_forContext")
            {
                // We have the following children:
                // Child #0: test (key expression)
                // Child #1: ":"
                // Child #2: test (value expression)
                // Child #3: comp_for
                TestVisitor keyVisitor = new TestVisitor(state);
                context.GetChild(0).Accept(keyVisitor);
                TestVisitor valueVisitor = new TestVisitor(state);
                context.GetChild(2).Accept(valueVisitor);
                // We use Linq for dict comprehension.
                state.output.usingDirs.Add("System.Linq");
                // Get the iterator expression.
                state.compForState = new CompForState();
                CompForVisitor compForVisitor = new CompForVisitor(state);
                context.GetChild(3).Accept(compForVisitor);
                for (int i = 0; i < compForVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(compForVisitor.result.tokens[i]);
                }
                result.tokens.Add(" select ");
                result.tokens.Add(state.compForState.iteratorExpr);
                
                result.tokens.Add(").ToDictionary(");
                result.tokens.Add(state.compForState.iteratorExpr);
                result.tokens.Add(" => ");
                result.tokens.Add(keyVisitor.result.ToString());
                result.tokens.Add(", ");
                result.tokens.Add(state.compForState.iteratorExpr);
                result.tokens.Add(" => ");
                result.tokens.Add(valueVisitor.result.ToString());
                result.tokens.Add(")");

                // We are done with compForState, we need to flush it.
                state.compForState = new CompForState();
            }
            else
            {

                result.tokens.Add("new Dictionary<dynamic, dynamic> {");
                state.varState.type = VarState.Types.Dictionary;
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
                    TestVisitor keyVisitor = new TestVisitor(state);
                    context.GetChild(j).Accept(keyVisitor);
                    j += 2;
                    TestVisitor valVisitor = new TestVisitor(state);
                    context.GetChild(j).Accept(valVisitor);
                    j += 2;
                    // Add a preceding comma to every item except to the first one.
                    if (j != 4)
                    {
                        result.tokens.Add(", ");
                    }
                    result.tokens.Add("{" + keyVisitor.result.ToString() + ", " +
                        valVisitor.result.ToString() + "}");
                }
                result.tokens.Add("}");
            }
        }
        // Case of a set.
        else
        {
            state.output.usingDirs.Add("System.Collections.Generic");

            state.varState.type = VarState.Types.HashSet;

            // This is a set comprehension.
            if (context.ChildCount == 2 && context.GetChild(1).GetType().ToString() ==
                "Python3Parser+Comp_forContext")
            {
                // We use Linq for set comprehension.
                state.output.usingDirs.Add("System.Linq");
                CompForVisitor compForVisitor = new CompForVisitor(state);
                context.GetChild(1).Accept(compForVisitor);
                
                for (int i = 0; i < compForVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(compForVisitor.result.tokens[i]);
                }
                result.tokens.Add(" select ");
                TestVisitor exprVisitor = new TestVisitor(state);
                // Force the collection of tokens only in the leftmost child where
                // the expression in the set comprehension is located.
                context.GetChild(0).Accept(exprVisitor);
                for (int i = 0; i < exprVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(exprVisitor.result.tokens[i]);
                }
                result.tokens.Add(").ToHashSet()");
            }
            else
            {
                result.tokens.Add("new HashSet<dynamic> {");
                int j = 0;
                // We assume that we have the following children:

                // Child 0: val_1
                // Child 1: ","
                // Child 2: val_2
                // ...

                while (j < n)
                {
                    TestVisitor valVisitor = new TestVisitor(state);
                    context.GetChild(j).Accept(valVisitor);
                    string value = valVisitor.result.ToString();

                    // In Python hash(1) == hash(True) and hash(0) == hash(False),
                    // so both such values cannot be present in a set.
                    bool isZero = (value == "0" || value == "false");
                    bool isOne = (value == "1" || value == "true");
                    if (isOne && !isOnePresent)
                    {
                        isOnePresent = true;
                    }
                    else if (isOne && isOnePresent)
                    {
                        // Skip this item.
                        j += 2;
                        continue;
                    }
                    else if (isZero && !isZeroPresent)
                    {
                        isZeroPresent = true;
                    }
                    else if (isZero && isZeroPresent)
                    {
                        // Skip this item.
                        j += 2;
                        continue;
                    }

                    // Add a preceding comma to every item except for the first one.
                    if (j != 0)
                    {
                        result.tokens.Add(", ");
                    }

                    result.tokens.Add("{" + value + "}");
                    j += 2;
                }
                result.tokens.Add("}");
            }
        }
        return result;
    }

}