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

        // Method/function call
        // We have the following children:
        // Child #0: atom
        // Child #1: trailer
        // Child #2: trailer

        // We can also have a call to the inner constructor:
        // self.B(arg), where B is an inner class.

        // We know that the only possible children are atom and trailer.
        // Check Python3Parser.g4 for the generating rule.
        if (context.ChildCount >= 3 &&
            context.GetChild(0).GetType().ToString() == "Python3Parser+AtomContext" &&
            context.GetChild(1).GetType().ToString() == "Python3Parser+TrailerContext")
        {
            // This is not a standalone expression.
            if (state.stmtState.isLocked == false)
            {
                state.stmtState.isStandalone = false;
                state.stmtState.isLocked = true;
            }

            int n = context.ChildCount;
            AtomVisitor atomVisitor = new AtomVisitor(state);
            List<MethodNameTrailerVisitor> methodNameTrailerVisitors = new List<MethodNameTrailerVisitor>();
            MethodArglistTrailerVisitor methodArglistTrailerVisitor = new MethodArglistTrailerVisitor(state);
            context.GetChild(0).Accept(atomVisitor);
            for (int i = 1; i < n - 1; ++i)
            {
                MethodNameTrailerVisitor newVisitor = new MethodNameTrailerVisitor(state);
                context.GetChild(i).Accept(newVisitor);
                methodNameTrailerVisitors.Add(newVisitor);
            }
            context.GetChild(n - 1).Accept(methodArglistTrailerVisitor);
            string varName = atomVisitor.result.ToString();


            // Method "append" on a list.
            // So far we don't consider nested expressions like a.b.append()
            if (state.output.currentClasses.Peek().currentFunctions.Peek().variables.ContainsKey(varName) &&
                state.output.currentClasses.Peek().currentFunctions.Peek().variables[varName] == VarState.Types.List &&
                methodNameTrailerVisitors[0].result.ToString() == ".append")
            {
                methodNameTrailerVisitors[0].result.tokens.Clear();
                methodNameTrailerVisitors[0].result.tokens.Add(".Add");
            }


            if (varName == "self")
            {
                varName = "";
                // Case of the inner constructor
                // self.B(arg)
                if (state.output.allClassesNames.Contains(methodNameTrailerVisitors[0].result.ToString().Remove(0, 1)))
                {
                    result.tokens.Add("new ");
                    // Drop the dot.
                    methodNameTrailerVisitors[0].result.tokens.RemoveAt(0);
                }
                // Expressions with self on the rhs.
                else
                {
                    result.tokens.Add("this");
                }

                for (int j = 0; j < methodNameTrailerVisitors.Count; ++j)
                {
                    for (int i = 0; i < methodNameTrailerVisitors[j].result.tokens.Count; ++i)
                    {
                        result.tokens.Add(methodNameTrailerVisitors[j].result.tokens[i]);
                    }
                }
                for (int i = 0; i < methodArglistTrailerVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(methodArglistTrailerVisitor.result.tokens[i]);
                }
            }
            // Case: BaseClass.__init__(self, arg1, arg2, ...)
            else if (state.output.currentClasses.Peek().parentClass != null &&
                state.output.currentClasses.Peek().parentClass.name == varName &&
                methodNameTrailerVisitors[0].result.ToString().Remove(0, 1) == "__init__")
            {
                // Copy each argument to the base constructor initializer list which
                // is translated to ... : base(arg1, arg2)
                // First arguments is skipped because it is "this"
                for (int i = 1; i < methodArglistTrailerVisitor.arguments.Count; ++i)
                {
                    string arg = methodArglistTrailerVisitor.arguments[i];
                    state.output.currentClasses.Peek().currentFunctions.Peek().
                        baseConstructorInitializerList.Add(arg);

                }
            }
            else
            {

                for (int i = 0; i < atomVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(atomVisitor.result.tokens[i]);
                }
                for (int j = 0; j < methodNameTrailerVisitors.Count; ++j)
                {
                    for (int i = 0; i < methodNameTrailerVisitors[j].result.tokens.Count; ++i)
                    {
                        result.tokens.Add(methodNameTrailerVisitors[j].result.tokens[i]);
                    }
                }
                for (int i = 0; i < methodArglistTrailerVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(methodArglistTrailerVisitor.result.tokens[i]);
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
            else if (name == "self")
            {
                name = "this";
            }
            // enumerate() - erase the name and remember the name in the state
            else if (name == "enumerate")
            {
                name = "";
                state.funcCallState.funcName = "enumerate";
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
            TrailerVisitor newVisitor = new TrailerVisitor(state);
            context.GetChild(1).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }

        return result;
    }
}