using System;
using System.Text;
using Antlr4.Runtime.Misc;
public class FuncdefVisitor : Python3ParserBaseVisitor<Function>
{
    public Function result;
    public State state;
    public FuncdefVisitor(State _state)
    {
        state = _state;
    }
    public override Function VisitFuncdef([NotNull] Python3Parser.FuncdefContext context)
    {
        state.output.currentClasses.Peek().currentFunctions.Push(new Function(state.output)); 

        // We assume that we have the following children:

        // Child 0: "def"
        // Child 1: function name
        // Child 2: parameters
        // Child 3: ":"
        // Child 4: suite

        TypedargslistVisitor parameterVisitor = new TypedargslistVisitor(state);
        context.GetChild(2).Accept(parameterVisitor);

        SuiteVisitor suiteVisitor = new SuiteVisitor(state);
        context.GetChild(4).Accept(suiteVisitor);

        result = state.output.currentClasses.Peek().currentFunctions.Pop();

        result.name = context.GetChild(1).ToString();

        
        // If the first paramter is "self" then we remove it.
        if (result.parameters.Count > 0 && result.parameters[0] == "self")
        {
            result.parameters.RemoveAt(0);
        }

        // Special case __init__ - constructor
        if (result.name == "__init__")
        {
            HandleInitMethod();
        }

        // Resolve each defaultParameter by null coalescing operator.

        if (state.output.currentClasses.Peek().name == "Program")
        {
            foreach (var defaultParameter in result.defaultParameters.Keys)
            {
                string nullCoalescingStr = "Program.";
                nullCoalescingStr += defaultParameter;

                // Declare defaultParameter as a static field.
                StringBuilder fieldDeclLine = new StringBuilder();
                fieldDeclLine.Append("static dynamic ");
                fieldDeclLine.Append(defaultParameter);
                fieldDeclLine.Append(" = null;");
                IndentedLine fieldDeclIndentedLine = new IndentedLine(fieldDeclLine.ToString(), 0);
                state.output.currentClasses.Peek().staticFieldDeclarations.lines.
                    Add(fieldDeclIndentedLine);
                state.output.currentClasses.Peek().staticFieldIdentifiers.Add(defaultParameter);


                nullCoalescingStr += " = ";
                nullCoalescingStr +=  defaultParameter;
                nullCoalescingStr += " ?? ";
                string value = result.defaultParameters[defaultParameter];
                var type = result.defaultParameterTypes[defaultParameter];
                if (type == VarState.Types.List || type == VarState.Types.ListComp || type == VarState.Types.ListInt)
                {
                    nullCoalescingStr += "Program.";
                    nullCoalescingStr += defaultParameter;
                    nullCoalescingStr += " ?? ";
                    nullCoalescingStr += result.defaultParameters[defaultParameter];
                }
                else if (type == VarState.Types.Int || type == VarState.Types.Double || type == VarState.Types.String)
                {
                    nullCoalescingStr += result.defaultParameters[defaultParameter];
                }
                else if (state.output.currentClasses.Peek().identifierToType.ContainsKey(value))
                {
                    var typeFromClass = state.output.currentClasses.Peek().identifierToType[value];
                    if (typeFromClass == VarState.Types.List || typeFromClass == VarState.Types.ListComp || typeFromClass == VarState.Types.ListInt)
                    {
                        nullCoalescingStr += "Program.";
                        nullCoalescingStr += defaultParameter;
                        nullCoalescingStr += " ?? ";
                        nullCoalescingStr += value;

                    }
                    else if (state.output.currentClasses.Peek().identifierToValueExpression.ContainsKey(value))
                    {
                        nullCoalescingStr += state.output.currentClasses.Peek().identifierToValueExpression[value];
                    }
                }
                nullCoalescingStr += ";";
                IndentedLine nullCoalescingLine = new IndentedLine(nullCoalescingStr, 0);
                result.statements.lines.Add(nullCoalescingLine);
            }
        }

        foreach (var line in suiteVisitor.result.lines)
        {
            result.statements.lines.Add(line);
        }

        // If the function is not internal, add it to the list of functions
        // in the class state.
        // Otherwise, add it to the list of functions in the current function.
        // If we are in the Program class, at the bottom there is always a Main
        // function.
        if (state.output.currentClasses.Peek().currentFunctions.Count > 1)
        {
            Function parentFunction = state.output.currentClasses.Peek().currentFunctions.Peek();

            // To be able to use parameters from the parent function.
            result.isStatic = false;

            parentFunction.internalFunctions.Add(result);
        }
        else
        {
            state.output.currentClasses.Peek().functions.Add(result);
        }
        result.parentClass = state.output.currentClasses.Peek();
        return result;
    }
    public void HandleInitMethod()
    {
        // Change the name to the name of the class (constructor).
        result.name = state.output.currentClasses.Peek().name;
        result.isConstructor = true;
        state.output.currentClasses.Peek().constructorSignatures
            [result.parameters.Count] = result;
    }


}