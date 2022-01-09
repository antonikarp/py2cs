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
        bool assignmentToIterationVariable = false;
        result = new LineModel();
        // This case handles variable declaration and initializtion.
        if (context.ChildCount == 3 && context.GetChild(1).ToString() == "=")
        {
            // This is not a standalone expression.
            state.stmtState.isStandalone = false;
            state.stmtState.isLocked = true;

            TestlistStarExprVisitor leftVisitor = new TestlistStarExprVisitor(state);
            TestlistStarExprVisitor rightVisitor = new TestlistStarExprVisitor(state);
            context.GetChild(0).Accept(leftVisitor);
            context.GetChild(2).Accept(rightVisitor);

            string identifier = "";
            string rhs = "";
            // Simple assignment like: "a = b"
            if (leftVisitor.result.expressions.Count == 1 && rightVisitor.result.expressions.Count == 1)
            {
                identifier = leftVisitor.result.expressions[0].ToString();
                rhs = rightVisitor.result.expressions[0].ToString();
            }
            // Comma seperated assignment like: "a, b = b, a"
            else
            {
                state.commaListAssignmentState = new CommaListAssignmentState();
                state.commaListAssignmentState.isActive = true;
                result.tokens.Add("dynamic ");
                ++state.output.currentClasses.Peek().currentFunctions.Peek().currentGeneratedTupleNumber;
                string tupleIdentifier = "_generated_tuple_" + state.output.currentClasses.Peek().currentFunctions.Peek().currentGeneratedTupleNumber;
                state.commaListAssignmentState.tupleIdentifier = tupleIdentifier;
                result.tokens.Add(tupleIdentifier);
                result.tokens.Add(" = (");
                for (int i = 0; i < rightVisitor.result.expressions.Count; ++i)
                {
                    if (i != 0)
                    {
                        result.tokens.Add(", ");
                    }
                    result.tokens.Add(rightVisitor.result.expressions[i].ToString());
                }
                result.tokens.Add(")");
                state.commaListAssignmentState.lhsExpressions = leftVisitor.result.expressions;
                // Stop here, we will add assignment to the lhs expressions after this statement.
                return result;

            }
            // If we have an assignment to the iteration variable, we need to
            // create a new variable. Only in for loop.
            if (state.loopState.loopType == LoopState.LoopType.ForLoop &&
                state.loopState.forStmtIterationVariable == identifier)
            {
                identifier = "_generated_" + identifier + "_" + state.loopState.generatedInBlockCount;
                state.loopState.nameForGeneratedVariable = identifier;
                assignmentToIterationVariable = true;
                ++state.loopState.generatedInBlockCount;
            }

            // Check if the variable has been already declared.
            // Or it can be declared as a field.
            string[] tokens = identifier.Split(".");

            // In case of assignmentToIterationVariable there will no assignments to a generated variable, because we generate
            if (!state.output.currentClasses.Peek().currentFunctions.Peek().variables.ContainsKey(identifier)
                && ((tokens.Length < 2) || ((tokens.Length >= 2) && (!state.output.currentClasses.Peek().fields.Contains(tokens[1])))))
            {
                // This is a case of declaration with initialization.
                switch (state.varState.type)
                {
                    case VarState.Types.List:
                        result.tokens.Add("List<dynamic> ");
                        break;
                    case VarState.Types.Dictionary:
                        result.tokens.Add("Dictionary<dynamic, dynamic> ");
                        break;
                    case VarState.Types.HashSet:
                        result.tokens.Add("HashSet<dynamic> ");
                        break;
                    // Due to CS1977 when trying to invoke .Where(lambda) in slices
                    // the type must be var.
                    case VarState.Types.ListComp:
                        result.tokens.Add("var ");
                        break;
                    // Type other (numeric) or tuple. Tuple is here, because
                    // it is inconvenient to explicity state the type like:
                    // (int, int) or (int, int, int) ...
                    case VarState.Types.Tuple:
                    case VarState.Types.Other:
                        result.tokens.Add("dynamic ");
                        break;

                }
                // Add the new variable to a respective scope.
                if (assignmentToIterationVariable)
                {
                    state.loopState.declaredIdentifiers.Add(identifier);
                }
                else
                {
                    state.output.currentClasses.Peek().currentFunctions.Peek().variables.Add(identifier, state.varState.type);
                }
            }
            // The following instructions are common for both cases (declaration
            // with initialization, assignment)
            result.tokens.Add(identifier);
            result.tokens.Add(" = ");

            // If it is a call to constructor, we need to add "new" keyword.
            string potentialConstructorCall = rhs;
            string[] splitBeforeLeftParan = potentialConstructorCall.Split('(');
            string[] identifiers = splitBeforeLeftParan[0].Split(".");
            bool isConstructorCall = true;
            foreach (var token in identifiers)
            {
                // For it to be a constructor each token delimited by a dot must
                // be a class name.
                if (!state.output.allClassesNames.Contains(token))
                {
                    isConstructorCall = false;
                }
            }
            if (isConstructorCall)
            {
                result.tokens.Add("new ");
                // If it is a parent class, we need to generate a constructor.
                // For now we assume that we don't have nested classes here:
                // Only p = Parent(arg1, arg2)
                // No: p = Module.Parent(arg1, arg2)
                if (identifiers.Length == 1 &&
                    state.output.namesToClasses[identifiers[0]].parentClass != null)
                {
                    // Remove the dangling right paran
                    splitBeforeLeftParan[1] = splitBeforeLeftParan[1].Remove(splitBeforeLeftParan[1].Length - 1);
                    // Remove spaces
                    splitBeforeLeftParan[1] = splitBeforeLeftParan[1].Replace(" ", "");
                    string[] arguments = splitBeforeLeftParan[1].Split(",");
                    List<VarState.Types> argumentTypes = new List<VarState.Types>();
                    foreach (string arg in arguments)
                    {
                        argumentTypes.Add(ParamTypeDeduction.Deduce(arg));
                    }
                    state.output.namesToClasses[identifiers[0]].GenerateConstructor(argumentTypes);
                }

            }
            result.tokens.Add(rhs);
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