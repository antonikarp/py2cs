using System;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;
public class AtomExprVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public AtomExprVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitAtom_expr([NotNull] Python3Parser.Atom_exprContext context)
    {

        result = new LineModel();

        // Check if we have a constructor call
        if (context.ChildCount >= 2)
        {
            AtomVisitor atomVisitor = new AtomVisitor(state);

            // We assume that 'atom' is the first child.
            // Temporarily lock the expression so that it won't become standalone.
            bool prevState = state.stmtState.isLocked;
            state.stmtState.isLocked = true;
            context.GetChild(0).Accept(atomVisitor);
            state.stmtState.isLocked = prevState;

            // When throwing an object of type Exception, this is also a constructor call.
            if (state.output.allClassesNames.Contains(atomVisitor.result.ToString()) ||
                atomVisitor.result.ToString() == "Exception")
            {
                state.trailerConstructorCheckState = new TrailerConstructorCheckState();
                bool isConstructorCall = true;
                if (context.ChildCount >= 3)
                {
                    int i = 1;
                    int n = context.ChildCount;
                    while (i < n - 1)
                    {
                        TrailerConstructorCheckVisitor newVisitor = new TrailerConstructorCheckVisitor(state);
                        context.GetChild(i).Accept(newVisitor);
                        isConstructorCall = isConstructorCall && newVisitor.isConstructor;
                        ++i;
                    }
                }
                if (isConstructorCall)
                {
                    // We have a constructor call. Add 'new' to the resulting string.
                    result.tokens.Add("new ");
                    // Save the name of the class in the state. It will be potentially
                    // used to give a type to a collection containing such elements.
                    // For now, we assume that are no collections of nested classes.
                    state.constructorCallState = new ConstructorCallState();
                    state.constructorCallState.isActive = true;
                    state.constructorCallState.name = atomVisitor.result.ToString();
                }
                // We are done with checking for a constructor call.
                state.trailerConstructorCheckState = new TrailerConstructorCheckState();
            }
        }

        // The function call can supply other functions as arguments, like:
        // "repeat(4)(myfunction)".

        if (context.ChildCount >= 3 &&
            context.GetChild(0).GetType().ToString() == "Python3Parser+AtomContext" &&
            context.GetChild(1).GetType().ToString() == "Python3Parser+TrailerContext" &&
            context.GetChild(1).ChildCount > 2)
        {
            // This is not a standalone expression.
            if (state.stmtState.isLocked == false)
            {
                state.stmtState.isStandalone = false;
                state.stmtState.isLocked = true;
            }

            AtomVisitor atomVisitor = new AtomVisitor(state);
            context.GetChild(0).Accept(atomVisitor);
            for (int j = 0; j < atomVisitor.result.tokens.Count; ++j)
            {
                result.tokens.Add(atomVisitor.result.tokens[j]);
            }
            int n = context.ChildCount;
            int i = 1;
            // Check if we have multiple slices (like: L[1:][:2]).
            bool areAllSlices = true;
            List<TrailerVisitor> trailerVisitors = new List<TrailerVisitor>();
            while (i < n)
            {
                TrailerVisitor newVisitor = new TrailerVisitor(state);
                context.GetChild(i).Accept(newVisitor);
                trailerVisitors.Add(newVisitor);
                areAllSlices = areAllSlices && newVisitor.isSlice;
                i += 1;
            }
            if (areAllSlices)
            {
                string identifier = atomVisitor.result.ToString();
                result.tokens.Clear();
                // Build expression like:
                // L[1:][:2] -> ListSlice.Get(ListSlice.Get(L, 1, null, null), null, 2, null);
                for (int j = 0; j < trailerVisitors.Count; ++j)
                {
                    result.tokens.Add("ListSlice.Get(");
                }
                for (int j = 0; j < trailerVisitors.Count; ++j)
                {
                    if (j == 0)
                    {
                        result.tokens.Add(identifier);
                        result.tokens.Add(", ");
                    }
                    else
                    {
                        result.tokens.Add(", ");
                    }
                    result.tokens.Add(trailerVisitors[j].sliceStart == null ? "null" : trailerVisitors[j].sliceStart);
                    result.tokens.Add(", ");
                    result.tokens.Add(trailerVisitors[j].sliceEnd == null ? "null" : trailerVisitors[j].sliceEnd);
                    result.tokens.Add(", ");
                    result.tokens.Add(trailerVisitors[j].sliceStride == null ? "null" : trailerVisitors[j].sliceStride);
                    result.tokens.Add(")");
                }
            }
            else
            {
                for (int j = 0; j < trailerVisitors.Count; ++j)
                {
                    for (int k = 0; k < trailerVisitors[j].result.tokens.Count; ++k)
                    {
                        result.tokens.Add(trailerVisitors[j].result.tokens[k]);
                    }
                }
            }
            return result;
        }


        // Method/function call
        // We have the following children:
        // Child #0: atom
        // Child #1: trailer (has 2 children: #0: dot, #1: name of the method).
        // Child #2: trailer

        // We can also have a call to the inner constructor:
        // self.B(arg), where B is an inner class.

        // We know that the only possible children are atom and trailer.
        // Check Python3Parser.g4 for the generating rule.
        else if (context.ChildCount >= 3 &&
        context.GetChild(0).GetType().ToString() == "Python3Parser+AtomContext" &&
        context.GetChild(1).GetType().ToString() == "Python3Parser+TrailerContext" &&
        context.GetChild(1).ChildCount == 2)
        {
            // This is not a standalone expression.
            if (state.stmtState.isLocked == false)
            {
                state.stmtState.isStandalone = false;
                state.stmtState.isLocked = true;
            }

            int n = context.ChildCount;
            AtomVisitor atomVisitor = new AtomVisitor(state);
            List<MethodArglistTrailerVisitor> trailerVisitors = new List<MethodArglistTrailerVisitor>();
            context.GetChild(0).Accept(atomVisitor);
            for (int i = 1; i < n; ++i)
            {
                MethodArglistTrailerVisitor newVisitor = new MethodArglistTrailerVisitor(state);
                context.GetChild(i).Accept(newVisitor);
                trailerVisitors.Add(newVisitor);
            }
            string varName = atomVisitor.result.ToString();

            // String comprehension - method join on the string.
            if (context.ChildCount == 3 &&
                trailerVisitors[0].result.ToString() == ".join")
            {
                result.tokens.Add("String.Join(");
                for (int i = 0; i < atomVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(atomVisitor.result.tokens[i]);
                }
                result.tokens.Add(", ");
                for (int i = 0; i < trailerVisitors[1].result.tokens.Count; ++i)
                {
                    result.tokens.Add(trailerVisitors[1].result.tokens[i]);
                }
                result.tokens.Add(")");
                return result;
            }

            // Method "isdigit" on a character.
            else if (context.ChildCount == 3 &&
                trailerVisitors[0].result.ToString() == ".isdigit")
            {
                result.tokens.Add("Char.IsDigit(");
                for (int i = 0; i < atomVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(atomVisitor.result.tokens[i]);
                }
                result.tokens.Add(")");
                return result;
            }

            // Method "append" on a list.
            // So far we don't consider nested expressions like a.b.append()
            // Split by the dot and take the last segment.
            // For instance -> Program.x -> x;
            string[] splitAfterDotTokens = varName.Split(".");
            string lastTokenAfterDot = splitAfterDotTokens[splitAfterDotTokens.Length - 1];
            if (state.output.currentClasses.Peek().currentFunctions.Peek().variables.ContainsKey(lastTokenAfterDot) &&
                state.output.currentClasses.Peek().currentFunctions.Peek().variables[lastTokenAfterDot] == VarState.Types.List &&
                trailerVisitors[0].result.ToString() == ".append")
            {
                trailerVisitors[0].result.tokens.Clear();
                trailerVisitors[0].result.tokens.Add(".Add");
                // Remember the type of the list which is a collection of objects
                // of class defined in the source.
                if (state.constructorCallState.isActive)
                {
                    state.output.currentClasses.Peek().currentFunctions.Peek().
                        listIdentifiersToClassNames[atomVisitor.result.ToString()] = state.constructorCallState.name;
                }
            }

            if (state.output.currentClasses.Peek().nameForSelf == varName && varName != "")
            {
               
                result.tokens.Add("this");

                for (int j = 0; j < trailerVisitors.Count; ++j)
                {
                    for (int i = 0; i <trailerVisitors[j].result.tokens.Count; ++i)
                    {
                        result.tokens.Add(trailerVisitors[j].result.tokens[i]);
                    }
                }
            }
            
            else
            {

                for (int i = 0; i < atomVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(atomVisitor.result.tokens[i]);
                }
                for (int j = 0; j < trailerVisitors.Count; ++j)
                {
                    for (int i = 0; i < trailerVisitors[j].result.tokens.Count; ++i)
                    {
                        result.tokens.Add(trailerVisitors[j].result.tokens[i]);
                    }
                }
            }
        }

        else if (context.atom() != null && context.atom().ChildCount == 1 && context.atom().NAME() != null)
        {
            // This is not a standalone expression.
            if (state.stmtState.isLocked == false)
            {
                state.stmtState.isStandalone = false;
                state.stmtState.isLocked = true;
            }

            AtomVisitor atomVisitor = new AtomVisitor(state);
            context.atom().Accept(atomVisitor);
            string name = atomVisitor.result.ToString();

            // Flush the FuncCallState
            state.funcCallState = new FuncCallState();

            // We might encounter a type cast (ex. int(), float()).
            // Flush the TypeCastFromNullCheckState
            state.typeCastFromNullCheckState = new TypeCastFromNullCheckState();

            // Reserved words, which cannot be used as identifiers.
            HashSet<string> reservedIdentifiers = new HashSet<string> { "private" };

            state.illegalKeywordArgumentsState = new IllegalKeywordArgumentsState();
            bool arePotentialIllegalKeywordArguments = true;
            foreach (var func in state.output.currentClasses.Peek().functions)
            {
                if (func.name == name && func.defaultParameters.Count > 0)
                {
                    arePotentialIllegalKeywordArguments = false;
                    break;
                }
            }
            state.illegalKeywordArgumentsState.isActive = arePotentialIllegalKeywordArguments;

            // Function call or field.         
            if (name == "print")
            {
                // In this case (print) the arguments are not changing, we can use the standard
                // TrailerVisitor for arguments. See one of the cases in this file.
                // (the one with "function call or field")
                name = "ConsoleExt.WriteLine";
            }
            else if (name == "range")
            {
                if (context.trailer() != null)
                {
                    // Here there are few cases so use a custom RangeTrailerVisitor.
                    state.output.usingDirs.Add("System.Linq");
                    result.tokens.Add("Enumerable.Range");
                    RangeTrailerVisitor newVisitor = new RangeTrailerVisitor(state);
                    context.GetChild(1).Accept(newVisitor);
                    for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
                    {
                        result.tokens.Add(newVisitor.result.tokens[j]);
                    }
                    // We don't consider the trailer anymore. End here.
                    return result;
                }
            }
            // Field.
            else if (state.output.currentClasses.Peek().nameForSelf == name && name != "")
            {
                name = "this";
            }
            // enumerate() - erase the name and remember the name in the state
            else if (name == "enumerate")
            {
                name = "";
                state.funcCallState.funcName = "enumerate";
            }
            // len() - erase the name and remember the name in the state
            else if (name == "len")
            {
                name = "";
                state.funcCallState.funcName = "len";
            }
            // input() -> this is equivalent to Console.ReadLine()
            // input(x) -> Console.Readline(); Console.Write(x) (another statement) 
            else if (name == "input")
            {
                if (state.toFloatConversionState.isActive)
                {
                    state.toFloatConversionState.isLocked = true;
                }
                name = "Console.ReadLine";
                state.inputState = new InputState();
                state.inputState.isActive = true;
            }
            // We have a type cast to int:
            // Or a cast to bool with an occurrence of an arithmetic expression ->
            // promotion to int.
            else if (name == "int" || (name == "bool" && state.promoteBoolToIntState.isAritmExpr))
            {
                state.typeCastFromNullCheckState.isActive = true;

                // Child #0: atom
                // Child #1: trailer
                if (context.GetChild(1).ChildCount == 2)
                {
                    // No arguments, we have: "int()", get the default value.
                    result.tokens.Add("default(int)");
                    return result;
                }
                else
                {
                    // One argument, use TrailerVisitor, we have: "int(a)".
                    result.tokens.Add("Convert.ToInt32");
                    state.floatToIntConversionState.isActive = true;
                    name = "";
                }
            }
            // We have a type cast to float:
            else if (name == "float")
            {
                state.typeCastFromNullCheckState.isActive = true;

                // Child #0: atom
                // Child #1: trailer
                if (context.GetChild(1).ChildCount == 2)
                {
                    // No arguments, we have: "float()", get the default value.
                    result.tokens.Add("default(float)");
                    return result;
                }
                else
                {
                    // One argument, use TrailerVisitor, we have: "float(a)".
                    result.tokens.Add("Convert.ToDouble");
                    state.toFloatConversionState.isActive = true;
                    name = "";
                }
            }
            // We have a type cast to bool:
            else if (name == "bool")
            {
                // Child #0: atom
                // Child #1: trailer
                if (context.GetChild(1).ChildCount == 2)
                {
                    // No arguments, we have: "bool()", get the default value.
                    result.tokens.Add("default(bool)");
                    return result;
                }
                else
                {
                    // One argument, use TrailerVisitor, we have: "bool(a)".
                    result.tokens.Add("Convert.ToBoolean");
                    name = "";
                    // Remember that we have a conversion to bool
                    // bool("False") == True, because its "False".Length > 0
                    state.stmtState.persistentFuncName = "bool";
                }
            }
            // Append "_0" do that the name of the identifier is legal in C#.
            else if (reservedIdentifiers.Contains(name))
            {
                name += "_0";
            }
            // Object or class attribute, we have the expression like,
            // for instance: 'A.x' or 'b.y'
            // Allow it only in case if we have an exception attribute.
            else if (context.ChildCount == 2 && context.trailer() != null &&
                context.GetChild(1).ChildCount >= 2 &&
                context.GetChild(1).GetChild(0).ToString() == "." &&
                (!state.exceptionAttributeState.isActive ||
                state.exceptionAttributeState.exceptionName != name))
            {
                throw new NotImplementedException("Object attribute or class attribute.", context.Start.Line);
            }
            else
            {

                // Check if the name of the function is the name of a previously defined
                // iterator.
                foreach (var func in state.output.currentClasses.Peek().functions)
                {
                    if (func.name == name && func.isEnumerable)
                    {
                        state.funcCallState.isIterator = true;
                        result.tokens.Add("new OnceEnumerable<dynamic>(");
                    }
                }

            }
            result.tokens.Add(name);
        }
        else if (context.atom() != null)
        {
            AtomVisitor atomVisitor = new AtomVisitor(state);
            context.atom().Accept(atomVisitor);
            for (int i = 0; i < atomVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(atomVisitor.result.tokens[i]);
            }

        }

        // Function call (ex. hello())
        // or field (ex. self.name)
        if (context.ChildCount == 2 && context.trailer() != null)
        {
            // Update each argument that is a function - it is Func<dynamic, dynamic>
            AtomVisitor atomVisitor = new AtomVisitor(state);
            context.GetChild(0).Accept(atomVisitor);
            string name = atomVisitor.result.ToString();
            // Go through each function in the currentFunctions stack.
            Stack<Function> currentFunctions = state.output.currentClasses.Peek().currentFunctions;
            List<Function> tempCurrentFunctions = new List<Function>();
            while (currentFunctions.Count > 0)
            {
                Function topFunction = currentFunctions.Pop();
                tempCurrentFunctions.Add(topFunction);
            }
            tempCurrentFunctions.Reverse();
            foreach (var function in tempCurrentFunctions)
            {
                for (int i = 0; i < function.parameters.Count; ++i)
                {
                    if (function.parameters[i] == name)
                    {
                        function.overridenParameterTypes[name] = "Func<dynamic, dynamic> ";
                    }
                }
                currentFunctions.Push(function);
            }
            tempCurrentFunctions.Clear();

            TrailerVisitor trailerVisitor = new TrailerVisitor(state);
            context.GetChild(1).Accept(trailerVisitor);

            // Tuple slice. Due to the fact that tuples are immutable, we need
            // to compute indices and construct the expression in the tool.
            // For instance: t[1:4] -> (t.Item2, t.Item3, t.Item4) 
            if (trailerVisitor.isSlice &&
                state.output.currentClasses.Peek().currentFunctions.Peek().tupleIdentifierToNumberOfElements.ContainsKey(name))
            {
                // We are not handling assignments where on the lhs there is a slice.
                if (state.lhsState.isLhsState)
                {
                    throw new NotImplementedException("Slice on the left-hand side of the assignment.", context.Start.Line);
                }

                int numberOfElements = state.output.currentClasses.Peek().
                    currentFunctions.Peek().tupleIdentifierToNumberOfElements[name];
                
                // So far, the slicing of tuples is handled only when the arguments in
                // the slice expression (start, end, stride) are numerical literals.
                int sliceStart, sliceEnd, sliceStride;

                // Remove any parentheses.
                if (trailerVisitor.sliceStart != null)
                {
                    trailerVisitor.sliceStart = trailerVisitor.sliceStart.Replace("(", "").Replace(")", "");
                }
                if (trailerVisitor.sliceEnd != null)
                {
                    trailerVisitor.sliceEnd = trailerVisitor.sliceEnd.Replace("(", "").Replace(")", "");
                }
                if (trailerVisitor.sliceStride != null)
                {
                    trailerVisitor.sliceStride = trailerVisitor.sliceStride.Replace("(", "").Replace(")", "");
                }

                Int32.TryParse(trailerVisitor.sliceStart, out sliceStart);
                Int32.TryParse(trailerVisitor.sliceEnd, out sliceEnd);
                Int32.TryParse(trailerVisitor.sliceStride, out sliceStride);
                int? sliceStartNullable, sliceEndNullable, sliceStrideNullable;
                sliceStartNullable = trailerVisitor.sliceStart == null ? null : sliceStart;
                sliceEndNullable = trailerVisitor.sliceEnd == null ? null : sliceEnd;
                sliceStrideNullable = trailerVisitor.sliceStride == null ? null : sliceStride;
                List<int> indices = GetIndicesTupleSlice.Get(numberOfElements,
                    sliceStartNullable, sliceEndNullable, sliceStrideNullable);

                result.tokens.Clear();
                result.tokens.Add("(");
                for (int i = 0; i < indices.Count; ++i)
                {
                    if (i != 0)
                    {
                        result.tokens.Add(", ");
                    }
                    // Index k corresponds to tuple item k+1
                    int tupleItemNumber = indices[i] + 1;
                    result.tokens.Add(name + ".Item" + tupleItemNumber);
                }
                result.tokens.Add(")");

            }
            else if (trailerVisitor.isSlice)
            {
                // We are not handling assignments where on the lhs there is a slice.
                if (state.lhsState.isLhsState)
                {
                    throw new NotImplementedException("Slice on the left-hand side of the assignment.", context.Start.Line);
                }

                // We have a slice:
                // a[start:stop:stride] -> ListSlice.Get(a, start, stop, stride)
                var identifier = result.ToString();
                result.tokens.Clear();
                result.tokens.Add("ListSlice.Get(");
                result.tokens.Add(identifier);
                result.tokens.Add(", ");
                result.tokens.Add(trailerVisitor.sliceStart == null ? "null" : trailerVisitor.sliceStart);
                result.tokens.Add(", ");
                result.tokens.Add(trailerVisitor.sliceEnd == null ? "null" : trailerVisitor.sliceEnd);
                result.tokens.Add(", ");
                result.tokens.Add(trailerVisitor.sliceStride == null ? "null" : trailerVisitor.sliceStride);
                result.tokens.Add(")");

                for (int i = 0; i < trailerVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(trailerVisitor.result.tokens[i]);
                }
            }
            else
            {
                for (int i = 0; i < trailerVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(trailerVisitor.result.tokens[i]);
                }
            }
            if (state.toFloatConversionState.isLocked)
            {
                state.toFloatConversionState.isLocked = false;
            }
            else if (state.toFloatConversionState.isActive)
            {
                bool isRemoved = false;
                if (!state.toFloatConversionState.isLocked &&
                    result.tokens.Count > 0 &&
                    result.tokens[result.tokens.Count - 1] == ")")
                {
                    result.tokens.RemoveAt(result.tokens.Count - 1);
                    isRemoved = true;
                }
                result.tokens.Add(", ");
                result.tokens.Add("System.Globalization.CultureInfo.InvariantCulture");
                state.toFloatConversionState = new ToFloatConversionState();
                if (isRemoved)
                {
                    result.tokens.Add(")");
                }
            }
        }
        // We are done with the function call. Flush the IllegalKeywordArgumentsState
        state.illegalKeywordArgumentsState = new IllegalKeywordArgumentsState();
        // The same with TypeCastFromNullCheckState
        state.typeCastFromNullCheckState = new TypeCastFromNullCheckState();
        return result;
    }
}