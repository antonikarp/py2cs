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
        // Expression is standalone:
        if (!state.stmtState.isLocked)
        {
            state.stmtState.isStandalone = true;
            state.stmtState.isLocked = true;
        }
        if (context.ChildCount == 1)
        {
            // Case of numeric literal
            if (context.NUMBER() != null)
            {
                string value = context.NUMBER().ToString();
                // In C# there are no octal literals. Convert a string representation
                // to int with base 8.
                if (value.StartsWith("0o") || value.StartsWith("0O"))
                {
                    value = value.Remove(0, 2);
                    result.tokens.Add("Convert.ToInt32(\"" + value + "\", 8)");
                }
                // Remove trailing dot.
                else if (value.EndsWith("."))
                {
                    value = value.Remove(value.Length - 1);
                    result.tokens.Add(value);
                }
                else
                {
                    result.tokens.Add(value);
                }
            }
            // Case of string literal
            else if (context.STRING().Length > 0)
            {
                string value = context.STRING().GetValue(0).ToString();

                // Replace single quotes with double quotes.
                if (value.StartsWith("'") && value.EndsWith("'"))
                {
                    value = value.Remove(value.Length - 1);
                    value = value.Remove(0, 1);
                    value = ("\"" + value + "\"");
                }
                result.tokens.Add(value);
            }
            // Function name
            else if (context.NAME() != null)
            {
                string name = context.NAME().ToString();

                // Check if we have a list of functions. First the type must be set to List.
                // Then (here) we encounter a function and set the type to ListFunc
                if (state.varState.type == VarState.Types.List &&
                    state.output.currentClasses.Peek().functionToSignature.ContainsKey(name))
                {
                    state.varState.type = VarState.Types.ListFunc;
                    state.varState.funcSignature = state.output.currentClasses.Peek().functionToSignature[name];
                }

                // Replace default variable identifier by a static field.
                if (state.output.currentClasses.Peek().name == "Program" &&
                    state.output.currentClasses.Peek().currentFunctions.Peek().defaultParameters.ContainsKey(name))
                {
                    name = "Program." + name;
                }

                // Rename the changed function identifier.
                if (state.output.currentClasses.Peek().currentFunctions.Count > 0 &&
                    state.output.currentClasses.Peek().currentFunctions.Peek().changedFunctionIdentifiers.Contains(name))
                {
                    name = name + "_0";
                }

                // Replace the iteration variable with the generated name.
                if (state.loopState.loopType == LoopState.LoopType.ForLoop &&
                    state.loopState.nameForGeneratedVariable != "" &&
                    state.loopState.forStmtIterationVariable == name)
                {
                    result.tokens.Add(state.loopState.nameForGeneratedVariable);
                }
                else
                {

                    foreach (var kv in state.output.usedNamesFromImport)
                    {
                        var list = kv.Value;
                        if ((list.IndexOf(name) != -1) && (list.IndexOf(name) != (list.Count - 1)))
                        {
                            // The alias needs to be updated to the last valid.
                            name = list[list.Count - 1];
                        }
                    }
                    if (state.output.usedNamesFromImport.Count == 0)
                    {
                        if (state.output.currentClasses.Peek().currentFunctions.Count > 0 &&
                            state.output.currentClasses.Peek().currentFunctions.Peek().identifiersReferringToGlobal.Contains(name))
                        {
                            // Example: @@@{x, <line_number>}
                            // Resolve identifier with "@@@" later.
                            // If it cannot be resolved then use <line_number> for the
                            // error message.
                            name = "@@@{" + name + "," + context.Start.Line + "}";
                            // Activate the respective state (to be used in ExprStmt).
                            state.varReferringToGlobalState = new VarReferringToGlobalState();
                            state.varReferringToGlobalState.isActive = true;
                        }
                        
                    }
                    result.tokens.Add(name);
                }


            }
            else if (context.TRUE() != null)
            {
                result.tokens.Add("true");
            }
            else if (context.FALSE() != null)
            {
                result.tokens.Add("false");
            }
            // For now, "None" is translated to "null".
            else if (context.NONE() != null)
            {
                result.tokens.Add("null");
            }
        }
        // Expression surrounded by parenthesis.
        else if (context.ChildCount == 3 &&
            context.GetChild(0).ToString() == "(" &&
            context.GetChild(2).ToString() == ")")
        {
            // If it exists, get a generator expression.
            CompForVisitor compForVisitor = new CompForVisitor(state);
            context.GetChild(1).Accept(compForVisitor);

            // Generator expression not found.
            if (compForVisitor.visited == false)
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
            else
            {
                // We use Linq for generators.
                state.output.usingDirs.Add("System.Linq");
                for (int i = 0; i < compForVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(compForVisitor.result.tokens[i]);
                }
                result.tokens.Add(" select ");
                TestVisitor exprVisitor = new TestVisitor(state);
                // Force the collection of tokens only in the leftmost child where
                // the expression in the list comprehension is located.
                context.GetChild(1).GetChild(0).Accept(exprVisitor);
                for (int i = 0; i < exprVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(exprVisitor.result.tokens[i]);
                }
                // Override the type. If tuple is returned, type 'Tuple'
                // cannot propagate to the 'for' loop.
                state.varState.type = VarState.Types.Other;
                result.tokens.Add(")");
            }
        }
        // Non-empty list
        else if (context.ChildCount == 3 &&
            context.GetChild(0).ToString() == "[" &&
            context.GetChild(2).ToString() == "]")
        {
            // If it exists, get a list comprehension.
            CompForVisitor compForVisitor = new CompForVisitor(state);
            context.GetChild(1).Accept(compForVisitor);

            // We use List from System.Collections.Generic
            state.output.usingDirs.Add("System.Collections.Generic");

            // List comprehension not found.
            if (compForVisitor.visited == false)
            {
                result.tokens.Add("new List<dynamic> {");
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
            else
            {
                // We use Linq for list comprehension
                state.output.usingDirs.Add("System.Linq");
                for (int i = 0; i < compForVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(compForVisitor.result.tokens[i]);
                }
                result.tokens.Add(" select ");
                TestVisitor exprVisitor = new TestVisitor(state);
                // Force the collection of tokens only in the leftmost child where
                // the expression in the list comprehension is located.
                context.GetChild(1).GetChild(0).Accept(exprVisitor);
                for (int i = 0; i < exprVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(exprVisitor.result.tokens[i]);
                }
                // Override type to be ListComp.
                state.varState.type = VarState.Types.ListComp;
                result.tokens.Add(").ToList()");
            }
        }
        // Empty list
        else if (context.ChildCount == 2 &&
            context.GetChild(0).ToString() == "[" &&
            context.GetChild(1).ToString() == "]")
        {
            // We use List from System.Collections.Generic
            state.output.usingDirs.Add("System.Collections.Generic");
            result.tokens.Add("new List<dynamic> {}");
        }
        // Empty dictionary
        else if (context.ChildCount == 2 &&
            context.GetChild(0).ToString() == "{" &&
            context.GetChild(1).ToString() == "}")
        {
            // We use Dictionary from System.Collections.Generic
            state.output.usingDirs.Add("System.Collections.Generic");
            result.tokens.Add("new Dictionary<dynamic, dynamic>{}");
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