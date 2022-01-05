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

            state.output.currentClasses.Peek().currentFunctions.Peek().parameters.Add(newParameter);
            // The parameter is also a valid variable in the function, so we need
            // to reserve the name.
            state.output.currentClasses.Peek().currentFunctions.Peek().variables[newParameter] = VarState.Types.Other;
            // We have a default parameter.
            if (i + 2 < n && context.GetChild(i + 1).ToString() == "=")
            {
                TestVisitor defaultValueVisitor = new TestVisitor(state);
                context.GetChild(i + 2).Accept(defaultValueVisitor);
                string value = defaultValueVisitor.result.ToString();
                state.output.currentClasses.Peek().currentFunctions.Peek().defaultParameters[newParameter] = value;
                VarState.Types defaultParameterDeducedType = ParamTypeDeduction.Deduce(value);
                state.output.currentClasses.Peek().currentFunctions.Peek().defaultParameterTypes[newParameter] = defaultParameterDeducedType;
                // Update the type of the variable
                state.output.currentClasses.Peek().currentFunctions.Peek().variables[newParameter] = defaultParameterDeducedType;

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