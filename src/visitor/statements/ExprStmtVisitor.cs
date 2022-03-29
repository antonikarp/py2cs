using System.Text;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        // If there is a declaration in the Main method it needs to be
        // a static member declaration.
        bool isStaticMemberDeclaration = false;

        result = new LineModel();
        // This case handles variable declaration and initializtion.
        if (context.ChildCount == 3 && context.GetChild(1).ToString() == "=")
        {
            // This is not a standalone expression.
            state.stmtState.isStandalone = false;
            state.stmtState.isLocked = true;

            TestlistStarExprVisitor leftVisitor = new TestlistStarExprVisitor(state);
            TestlistStarExprVisitor rightVisitor = new TestlistStarExprVisitor(state);
            state.lhsTupleState = new LhsTupleState();

            // Block casts to object if a tuple is on lhs.
            state.lhsState = new LhsState();
            state.lhsState.isLhsState = true;

            context.GetChild(0).Accept(leftVisitor);
            // Check if there is a tuple on the lhs. This bool value is set in AtomExprVisitor.
            bool isTupleOnLhs = state.lhsTupleState.isTupleOnLhs;

            // Flush the LhsTupleState and LhsState
            state.lhsTupleState = new LhsTupleState();
            state.lhsState = new LhsState();

            context.GetChild(2).Accept(rightVisitor);
            bool isTupleAssignment = false;

            string lhs = "";
            string rhs = "";
            if (leftVisitor.result.expressions.Count == 1 && rightVisitor.result.expressions.Count == 1)
            {
                isTupleAssignment = isTupleOnLhs;
                rhs = rightVisitor.result.expressions[0];
            }
            else if (leftVisitor.result.expressions.Count > 1 && rightVisitor.result.expressions.Count == 1)
            {
                isTupleAssignment = true;
                rhs = rightVisitor.result.expressions[0];
            }
            else if (rightVisitor.result.expressions.Count > 1)
            {
                isTupleAssignment = true;
                rhs += "(";
                for (int i = 0; i < rightVisitor.result.expressions.Count; ++i)
                {
                    if (i != 0)
                    {
                        rhs += ", ";
                    }
                    rhs += rightVisitor.result.expressions[i];
                }
                rhs += ")";
            }
            if (isTupleAssignment)
            {
                if (leftVisitor.result.expressions.Count == 1)
                {
                    // Todo: unpack the tuple on the lhs and add "dynamic" to
                    // initialized variables.
                    lhs = leftVisitor.result.expressions[0];
                }
                else
                {
                    lhs += "(";
                    // Build lhs here with possible initializations of variables.  
                    for (int i = 0; i < leftVisitor.result.expressions.Count; ++i)
                    {
                        if (i != 0)
                        {
                            lhs += ", ";
                        }
                        // Initialize new variables.

                        // The expression can have a form: 'a[2]'. To get the name of the variable, we need to split it
                        // by '[' and take the first token.
                        string[] potentialSubscriptionTokensTuple = leftVisitor.result.expressions[i].Split("[");

                        string cleanedIdentifierTuple = potentialSubscriptionTokensTuple[0];

                        if (!state.output.currentClasses.Peek().currentFunctions.Peek().
                            variables.ContainsKey(cleanedIdentifierTuple))
                        {
                            lhs += "dynamic ";
                            state.output.currentClasses.Peek().currentFunctions.Peek().variables.Add(cleanedIdentifierTuple, VarState.Types.Other);
                            lhs += cleanedIdentifierTuple;
                        }
                        // We don't declare the variable. The subscription (ex. 'a[2]') must remain.
                        else
                        {
                            lhs += leftVisitor.result.expressions[i];
                        }
                        
                    }
                    lhs += ")";
                }

                result.tokens.Add(lhs);
                result.tokens.Add(" = ");
                result.tokens.Add(rhs);

                // We are done at this point.
                return result;
            }
            else
            {
                lhs = leftVisitor.result.expressions[0].ToString();
            }

            if (state.varState.type == VarState.Types.Tuple)
            {
                // If it is a tuple then store the number of elements to be
                // used when computing slices.
                state.output.currentClasses.Peek().currentFunctions.Peek().
                    tupleIdentifierToNumberOfElements[lhs] = state.tupleState.numberOfElements;
                // Flush the TupleState.
                state.tupleState = new TupleState();
            }

            // If we have an assignment to the iteration variable, we need to
            // create a new variable. Only in for loop.
            if (state.loopState.loopType == LoopState.LoopType.ForLoop &&
            state.loopState.forStmtIterationVariable == lhs)
            {
                lhs = "_generated_" + lhs + "_" + state.loopState.generatedInBlockCount;
                state.loopState.nameForGeneratedVariable = lhs;
                ++state.loopState.generatedInBlockCount;
            }

            if (state.varReferringToGlobalState.isActive)
            {
                Regex rx = new Regex("@@@{(.*),(.*)}");
                MatchCollection matches = rx.Matches(lhs);
                if (matches.Count > 0)
                {
                    string identifier = matches[0].Groups[1].Value;
                    // Make a new static field declaration only if there is no such static field identifier.
                    // Otherwise the function ResolveGlobalVariables() will resolve the string: @@@{x, y}
                    if (!state.output.currentClasses.Peek().staticFieldIdentifiers.Contains(identifier))
                    {
                        lhs = identifier;
                        isStaticMemberDeclaration = true;
                        state.varReferringToGlobalState.isActive = false;
                    }
                }
            }


            // Check if the variable has been already declared.
            // Or it can be declared as a field.
            string[] tokens = lhs.Split(".");

            // The lhs can have a form: 'a[2]'. To get the name of the variable, we need to split it
            // by '[' and take the first token.
            string[] potentialSubscriptionTokens = lhs.Split("[");

            string cleanedIdentifier = potentialSubscriptionTokens[0];

            // In case of assignmentToIterationVariable there will no assignments to a generated variable, because we generate the name.
            if (state.output.currentClasses.Peek().currentFunctions.Count > 0 &&
                !state.output.currentClasses.Peek().currentFunctions.Peek().variables.ContainsKey(cleanedIdentifier)
                && ((tokens.Length < 2) || ((tokens.Length >= 2) && (!state.output.currentClasses.Peek().fields.Contains(tokens[1])))) &&
                (!state.varReferringToGlobalState.isActive) &&
                // We exclude the situation where the function is "Main" and such static field has already been declared.
                (state.output.currentClasses.Peek().currentFunctions.Peek().name != "Main" ||
                !state.output.currentClasses.Peek().staticFieldIdentifiers.Contains(cleanedIdentifier))
                // We do not redeclare identifiers marked as 'nonlocal'
                && !state.output.currentClasses.Peek().currentFunctions.Peek().identifiersReferringToNonlocal.Contains(cleanedIdentifier))
            {
                // This is a case of declaration with initialization. We cannot be inside the scope of any loop.
                if (state.output.currentClasses.Peek().currentFunctions.Peek().name == "Main" &&
                    state.loopState.loopType == LoopState.LoopType.NoLoop)
                {
                    isStaticMemberDeclaration = true;
                }
                switch (state.varState.type)
                {
                    case VarState.Types.ListFunc:
                        string newToken = "List<";
                        newToken += state.varState.funcSignature;
                        newToken += ">";
                        result.tokens.Add(newToken);
                        // On the rhs we have: "new List<dynamic>". We need
                        // to change it to: "new List<Func<...>>".
                        rhs = rhs.Replace("List<dynamic>", newToken);
                        break;
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
                    // Important: for now, it cannot be declared as a static member (due to
                    //            usage of 'var' so here we make an exception and
                    //            proceed with the declaration in the function.
                    case VarState.Types.ListComp:
                        result.tokens.Add("var ");
                        isStaticMemberDeclaration = false;
                        break;
                    // Type other (numeric) or tuple. Tuple is here, because
                    // it is inconvenient to explicity state the type like:
                    // (int, int) or (int, int, int) ...
                    case VarState.Types.Tuple:
                    case VarState.Types.Other:
                        result.tokens.Add("dynamic ");
                        // Try to check for type int/double/string
                        state.varState.type = ParamTypeDeduction.Deduce(rhs);
                        break;

                }

                // Add the new variable to a respective scope.
                // Todo: unify loopState and scopeState.
                if (state.loopState.loopType != LoopState.LoopType.NoLoop)
                {
                    state.loopState.declaredIdentifiers.Add(lhs);
                    state.output.currentClasses.Peek().currentFunctions.Peek().hiddenIdentifiers.Add(lhs);
                }
                else if (state.scopeState.isActive)
                {
                    state.output.currentClasses.Peek().currentFunctions.Peek().hiddenIdentifiers.Add(lhs);
                }
                else
                {
                    state.output.currentClasses.Peek().currentFunctions.Peek().variables.Add(lhs, state.varState.type);
                }
            }
            

            // Check if lhs is a function identifier. If so, it needs to be renamed
            foreach (var func in state.output.currentClasses.Peek().functions)
            {
                if (lhs == func.name && state.output.currentClasses.Peek().currentFunctions.Count > 0)
                {
                    state.output.currentClasses.Peek().currentFunctions.Peek().changedFunctionIdentifiers.Add(lhs);
                    lhs = lhs + "_0";
                    break;
                }
            }

            result.tokens.Add(lhs);

            // Move the declaration to the field declarations.
            if (isStaticMemberDeclaration && !state.output.currentClasses.Peek().staticFieldIdentifiers.Contains(lhs))
            {
                StringBuilder fieldDeclLine = new StringBuilder();
                fieldDeclLine.Append("static ");
                for (int i = 0; i < result.tokens.Count; ++i)
                {
                    fieldDeclLine.Append(result.tokens[i]);
                }
                fieldDeclLine.Append(" = null;");
                IndentedLine fieldDeclIndentedLine = new IndentedLine(fieldDeclLine.ToString(), 0);
                state.output.currentClasses.Peek().staticFieldDeclarations.lines.
                    Add(fieldDeclIndentedLine);
                state.output.currentClasses.Peek().staticFieldIdentifiers.Add(lhs);

                // We have moved this declaration. Change the declaration to intialization.
                result = new LineModel();
                result.tokens.Add(lhs);
            }

            
            // The following instructions are common for both cases (declaration
            // with initialization, assignment)
            result.tokens.Add(" = ");

            
            result.tokens.Add(rhs);
            // Save the current value expression and its type. It can be used when there is a variable
            // as a default parameter.
            state.output.currentClasses.Peek().identifierToValueExpression[lhs] = rhs;
            state.output.currentClasses.Peek().identifierToType[lhs] = state.varState.type;

            return result;
        }
        // Chained assignment, for instance: 'x = y = z = 1' 
        else if (context.ChildCount > 3 && context.GetChild(1).ToString() == "=")
        {
            // We assume that we have the following children:
            // Child #0: testlist_star_expr
            // Child #1: "="
            // Child #2: testlist_star_expr
            // Child #3: "="
            // Child #4: testlist_star_expr

            // This is not a standalone expression.
            state.stmtState.isStandalone = false;
            state.stmtState.isLocked = true;

            int n = context.ChildCount;
            int i = 0;
            while (i < n)
            {
                TestlistStarExprVisitor newVisitor = new TestlistStarExprVisitor(state);
                context.GetChild(i).Accept(newVisitor);
                string identifier = newVisitor.result.expressions[0];

                // If there is a declaration in Main, move the declaration to the field declarations.
                // Skip the last expression
                if (state.output.currentClasses.Peek().currentFunctions.Peek().name == "Main" &&
                    state.loopState.loopType == LoopState.LoopType.NoLoop
                    && !state.output.currentClasses.Peek().staticFieldIdentifiers.Contains(identifier) &&
                    i != n - 1)
                {
                    StringBuilder fieldDeclLine = new StringBuilder();
                    fieldDeclLine.Append("static dynamic ");
                    fieldDeclLine.Append(identifier);
                    fieldDeclLine.Append(" = null;");
                    IndentedLine fieldDeclIndentedLine = new IndentedLine(fieldDeclLine.ToString(), 0);
                    state.output.currentClasses.Peek().staticFieldDeclarations.lines.
                        Add(fieldDeclIndentedLine);
                    state.output.currentClasses.Peek().staticFieldIdentifiers.Add(identifier);
                }
                if (i != 0)
                {
                    result.tokens.Add(" = ");
                }
                result.tokens.Add(identifier);
                i += 2;
            }
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
            string rawIdentifier = leftVisitor.result.ToString();
            // There is a possibility that the identifer has a form: @@@{x}.
            // We need to strip these extra characters, so that we get 'x'.
            Regex rx = new Regex("@@@{(.*),(.*)}");
            MatchCollection matches = rx.Matches(rawIdentifier);
            if (matches.Count > 0)
            {
                rawIdentifier = matches[0].Groups[1].Value;
            }
            // Handle subscriptions like: L[3] += 2
            else
            {
                rawIdentifier = rawIdentifier.Split("[")[0];
            }
            // We cannot have augmented assignment to a variable we haven't declared.
            if (!state.output.currentClasses.Peek().currentFunctions.Peek().variables.ContainsKey(rawIdentifier) &&
                !state.output.currentClasses.Peek().staticFieldIdentifiers.Contains(rawIdentifier) &&
                !state.output.currentClasses.Peek().currentFunctions.Peek().hiddenIdentifiers.Contains(rawIdentifier) &&
                !state.output.currentClasses.Peek().currentFunctions.Peek().identifiersReferringToGlobal.Contains(rawIdentifier) &&
                !state.output.currentClasses.Peek().currentFunctions.Peek().identifiersReferringToNonlocal.Contains(rawIdentifier))
            {
                throw new IncorrectInputException("Illegal augmented assignment.", context.Start.Line);
            }
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