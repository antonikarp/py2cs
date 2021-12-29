using System;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;

public class ExprStmtVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public ExprStmtVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitExpr_stmt([NotNull] Python3Parser.Expr_stmtContext context)
    {
        result = new LineModel();
        // This case handles variable declaration and initializtion.
        // Todo: handle assignment.
        if (context.ChildCount == 3 && context.GetChild(1).ToString() == "=")
        {
            TestVisitor leftVisitor = new TestVisitor(state);
            TestVisitor rightVisitor = new TestVisitor(state);
            context.GetChild(0).Accept(leftVisitor);
            context.GetChild(2).Accept(rightVisitor);
            // Check if the variable has been already declared.
            if (!state.funcState.variables.ContainsKey(leftVisitor.result.ToString()))
            {
                // This is a case of declaration with initialization.
                switch (state.varState.type)
                {
                    case VarState.Types.List:
                        result.tokens.Add("List<object> ");
                        break;
                    case VarState.Types.Dictionary:
                        result.tokens.Add("Dictionary<object, object> ");
                        break;
                    case VarState.Types.HashSet:
                        result.tokens.Add("HashSet<object>");
                        break;
                    // Type other (numeric) or tuple. Tuple is here, because
                    // it is inconvenient to explicity state the type like:
                    // (int, int) or (int, int, int) ...
                    case VarState.Types.Tuple:
                    case VarState.Types.Other:
                        result.tokens.Add("dynamic ");
                        break;

                }
                state.funcState.variables.Add(leftVisitor.result.ToString(), state.varState.type);
            }
            // The following instructions are common for both cases (declaration
            // with initialization, assignment)
            for (int i = 0; i < leftVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(leftVisitor.result.tokens[i]);
            }
            result.tokens.Add(" = ");
            for (int i = 0; i < rightVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(rightVisitor.result.tokens[i]);
            }
            return result;

        }
        // Augmented assignment
        else if (context.ChildCount == 3 &&
            context.GetChild(1).GetType().ToString() == "Python3Parser+AugassignContext")
        {
            TestVisitor leftVisitor = new TestVisitor(state);
            AugAssignVisitor opVisitor = new AugAssignVisitor(state);
            TestVisitor rightVisitor = new TestVisitor(state);      
            context.GetChild(0).Accept(leftVisitor);
            context.GetChild(1).Accept(opVisitor);
            context.GetChild(2).Accept(rightVisitor);
            for (int i = 0; i < leftVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(leftVisitor.result.tokens[i]);
            }
            result.tokens.Add(" " + opVisitor.result.value + " ");
            for (int i = 0; i < rightVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(rightVisitor.result.tokens[i]);
            }
            return result;
        }
        else if (context.ChildCount == 1)
        {
            TestVisitor newVisitor = new TestVisitor(state);
            context.Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        return result;
    }

}