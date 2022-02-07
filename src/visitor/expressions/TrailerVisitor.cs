using Antlr4.Runtime.Misc;
using System;

// This visitor is used in translating:
// * function calls
// * subscriptions (ex. dict_a["key"])

public class TrailerVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public bool isSlice;
    public string sliceStart;
    public string sliceEnd;
    public string sliceStride;
    public TrailerVisitor(State _state)
    {
        state = _state;
        isSlice = false;
        sliceStart = null;
        sliceEnd = null;
        sliceStride = null;
    }

    public override LineModel VisitTrailer([NotNull] Python3Parser.TrailerContext context)
    {
        result = new LineModel();
        // If the VarState.Type is ListFunc, change it to List, because it is a function call.
        if (state.varState.type == VarState.Types.ListFunc)
        {
            state.varState.type = VarState.Types.List;
            state.varState.funcSignature = "";
        }

        // Function call - some parameters
        if (context.ChildCount == 3 && context.GetChild(0).ToString() == "(" &&
            context.GetChild(2).ToString() == ")" &&
            context.GetChild(1).GetType().ToString() == "Python3Parser+ArglistContext")
        {
            string funcName = "";
            bool isTypeCastFromNullCheck = state.typeCastFromNullCheckState.isActive;
            // Remember funcName here. When creating new ArgumentVisitor in the following code
            // the state is flushed, because there is 'atom_expr' as a child of argument.
            if (state.funcCallState.funcName == "enumerate")
            {
                funcName = "enumerate";

            }
            else if (state.funcCallState.funcName == "len")
            {
                funcName = "len";
            }
            else if (state.funcCallState.funcName == "input")
            {
                funcName = "input";
            }

            result.tokens.Add("(");
            // We assume that we have the following children:
            // Child #0: argument_0
            // Child #1: ","
            // Child #2: argument_1
            // ...
            int n = context.GetChild(1).ChildCount;
            int i = 0;

            // If we have the 'input' function, then we have only one argument:
            // Save it to the inputState and not to result, because Console.ReadLine()
            // does not take any arguments.
            if (n == 1 && state.inputState.isActive)
            {
                ArgumentVisitor newVisitor = new ArgumentVisitor(state);
                context.GetChild(1).GetChild(0).Accept(newVisitor);
                state.inputState.argument = newVisitor.result.ToString();
            }
            else if (n == 1 && state.floatToIntConversionState.isActive)
            {
                ArgumentVisitor newVisitor = new ArgumentVisitor(state);
                context.GetChild(1).GetChild(0).Accept(newVisitor);
                string value = newVisitor.result.ToString();

                // Conversions: int(None), float(None) are illegal.
                if (value == "null" && isTypeCastFromNullCheck)
                {
                    throw new IncorrectInputException("Illegal cast from None.");
                }
                if (state.output.currentClasses.Peek().currentFunctions.Peek().variables.ContainsKey(value) &&
                        state.output.currentClasses.Peek().currentFunctions.Peek().variables[value] == VarState.Types.Double)
                {
                    result.tokens.Add("Math.Floor(");
                    result.tokens.Add(value);
                    result.tokens.Add(")");
                    newVisitor.result.ToString();
                }
                else
                {
                    result.tokens.Add(value);
                }

            }
            else
            {
                while (i < n)
                {
                    if (i != 0)
                    {
                        result.tokens.Add(", ");
                    }
                    ArgumentVisitor newVisitor = new ArgumentVisitor(state);
                    context.GetChild(1).GetChild(i).Accept(newVisitor);
                    for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
                    {
                        result.tokens.Add(newVisitor.result.tokens[j]);
                    }

                    // Conversions: int(None), float(None) are illegal.
                    string value = newVisitor.result.ToString();
                    if (value == "null" && isTypeCastFromNullCheck)
                    {
                        throw new IncorrectInputException("Illegal cast from None.");
                    }
                    i += 2;
                }
            }
            result.tokens.Add(")");

            // The case of 'enumerate'. We use a Linq query to create a list of tuples.
            if (funcName == "enumerate")
            {
                state.output.usingDirs.Add("System.Linq");
                result.tokens.Add(".Select((p1, p2) => ValueTuple.Create((object)p2, (object)p1))");

            }
            // The case of 'len'. Use the Count property. For now we assume that the argument is a list.
            else if (funcName == "len")
            {
                result.tokens.Add(".Count");
            }
            // If we have an expression coming from the input, it is string, and
            // when we have an active conversion to bool, we need to append .Length > 0
            // bool("False") == True, because "False".Length > 0
            // Todo: do the same when the varState.Type is string.
            else if (state.stmtState.persistentFuncName == "bool" && state.inputState.isActive)
            {
                result.tokens.Add(".Length > 0");

                // We are done with this attribute.
                state.stmtState.persistentFuncName = "";
            }

            // We are done with FuncCall state. We need to flush it.
            state.funcCallState = new FuncCallState();

            // Flush the FloatToIntConversionState
            state.floatToIntConversionState = new FloatToIntConversionState();

        }
        // Subscription and slices
        else if (context.ChildCount == 3 && context.GetChild(0).ToString() == "[" &&
            context.GetChild(2).ToString() == "]")
        {

            Subscript_Visitor newVisitor = new Subscript_Visitor(state);
            context.GetChild(1).Accept(newVisitor);
            string value = newVisitor.result.ToString();
            // Remove any parentheses.
            value = value.Replace("(", "");
            value = value.Replace(")", "");
            string[] parts = value.Split(':');

            // We have the case of a subscription: (e.g. a[1])
            if (parts.Length == 1)
            {
                result.tokens.Add("[");
                CheckForIllegalDoubleIndex(value);
                result.tokens.Add(value);
                result.tokens.Add("]");
            }
            else
            {
                isSlice = true;
                state.output.library.CommitListSlice();
                if (parts.Length == 2)
                {
                    sliceStart = (parts[0] == "" ? null : parts[0]);
                    sliceEnd = (parts[1] == "" ? null : parts[1]);
                }
                else if (parts.Length == 3)
                {
                    sliceStart = (parts[0] == "" ? null : parts[0]);
                    sliceEnd = (parts[1] == "" ? null : parts[1]);
                    sliceStride = (parts[2] == "" ? null : parts[2]);
                }
                if (sliceStart != null)
                {
                    CheckForIllegalDoubleIndex(sliceStart);
                }
                if (sliceEnd != null)
                {
                    CheckForIllegalDoubleIndex(sliceEnd);
                }
                if (sliceStride != null)
                {
                    CheckForIllegalDoubleIndex(sliceStride);
                }
            }
        }
        // Function call - no parameters
        else if (context.ChildCount == 2 && context.GetChild(0).ToString() == "(" &&
            context.GetChild(1).ToString() == ")")
        {
            result.tokens.Add("()");
            
            // Case of iterator
            if (state.funcCallState.isIterator)
            {
                result.tokens.Add(".GetEnumerator())");
            }
            // We are done with FuncCall state. We need to flush it.
            state.funcCallState = new FuncCallState();
        }
        // Field (ex. self.name)
        else if (context.ChildCount == 2 && context.GetChild(0).ToString() == ".")
        {
            result.tokens.Add(".");
            string name = context.GetChild(1).ToString();

            // Add a line to the field declarations.
            if (!state.output.currentClasses.Peek().fields.Contains(name))
            {
                string line = "public dynamic ";
                line += name;
                line += ";";
                state.output.currentClasses.Peek().fieldDecl.lines.Add(new IndentedLine(line, 0));

                // Add a name to the fields of the class.
                state.output.currentClasses.Peek().fields.Add(name);
            }

            result.tokens.Add(name);
        }

        return result;
    }
    private void CheckForIllegalDoubleIndex(string value)
    {
        int intValue;
        double doubleValue;
        if (!Int32.TryParse(value, out intValue) && Double.TryParse(value, out doubleValue))
        {
            throw new IncorrectInputException("Illegal double index.");
        }
    }

}