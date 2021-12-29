using System;
using System.Text;
using Antlr4.Runtime.Misc;
public class TypedargslistVisitor : Python3ParserBaseVisitor<Empty>
{
    public Empty result;
    public State state;

    public TypedargslistVisitor(State _state)
    {
        state = _state;
    }
    public override Empty VisitTypedargslist([NotNull] Python3Parser.TypedargslistContext context)
    {
        result = new Empty();

        // In case of positional parameters we have the following children:
        // Child #0: tfpdef_0
        // Child #1: ","
        // Child #2: tfpdef_1
        // Child #2: ","
        // ...

        // In case of default parameters we have the following children:
        // Child #0: tfpdef_0
        // Child #1: "="
        // Child #2: test
        // Child #3: ","
        // Child #4: tfpdef_1
        // ...
        int n = context.ChildCount;
        int i = 0;
        while (i < n)
        {
            TfpdefVisitor newVisitor = new TfpdefVisitor(state);
            context.GetChild(i).Accept(newVisitor);
            string newParameter = newVisitor.result.value.ToString();
            state.funcState.parameters.Add(newParameter);
            // We have a default parameter.
            if (i + 2 < n && context.GetChild(i + 1).ToString() == "=")
            {
                TestVisitor defaultValueVisitor = new TestVisitor(state);
                context.GetChild(i + 2).Accept(defaultValueVisitor);
                state.funcState.defaultParameters[newParameter] = defaultValueVisitor.result.ToString();
                i += 4;
            }
            // We have a positional parameter.
            else
            {
                i += 2;
            }
        }
        return result;
    }


}