using System;
using Antlr4.Runtime.Misc;

// This is a visitor used to compute an expression created with a logical operator "and".
// It traverses the parse tree from the node "and_test".

public class AndTestVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public AndTestVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitAnd_test([NotNull] Python3Parser.And_testContext context)
    {
        result = new LineModel();
        // If there is one child then it is a 'not_test' node.
        if (context.ChildCount == 1)
        {
            NotTestVisitor newVisitor = new NotTestVisitor(state);
            context.GetChild(0).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        // If there are more than one child then we have the following children:
        // Child #0: <expr1>
        // Child #1: and
        // Child #2: <expr2>
        // Child #3: ...

        // Generate a function only if we are not in the list comprehension, due
        // to the fact that the variable used there will be outside of scope
        // of the generated function.
        else if (context.ChildCount == 3 && state.listCompState.isActive == false)
        {
            // Expression is standalone:
            if (!state.stmtState.isLocked)
            {
                state.stmtState.isStandalone = true;
                state.stmtState.isLocked = true;
            }
            NotTestVisitor leftVisitor = new NotTestVisitor(state);
            context.GetChild(0).Accept(leftVisitor);
            NotTestVisitor rightVisitor = new NotTestVisitor(state);
            context.GetChild(2).Accept(rightVisitor);
            VarState.Types leftType = ParamTypeDeduction.Deduce(leftVisitor.result.ToString());

            // Surround the expression with parentheses.
            result.tokens.Add("(");

            // a and b is translated into
            //
            // var var_0 = a;
            // if (func(var_0))
            // {
            //      return b;
            // }
            // else
            // {
            //      return var_0;
            // }
            // where func is a type specific bool converter:
            // for list: .Count > 0, for int: != 0


            // Left-hand side is either an int or double
            // The expression "x and y" is equivalent to: x != 0 ? y : x
            ++state.output.currentClasses.Peek().currentGeneratedAndExpressionNumber;
            var num = state.output.currentClasses.Peek().currentGeneratedAndExpressionNumber;
            Function andExpressionFunction = new Function(state.output);
            andExpressionFunction.name = "GeneratedAndExpression" + num;
            andExpressionFunction.isPublic = false;
            andExpressionFunction.isVoid = false;
            andExpressionFunction.isStatic = false;
            andExpressionFunction.statements.lines.Add(new IndentedLine
                ("var var_0 = " + leftVisitor.result.ToString() + ";", 0));
            if (leftType == VarState.Types.Int || leftType == VarState.Types.Double)
            {
                andExpressionFunction.statements.lines.Add(new IndentedLine
                ("if (var_0 != 0)", 0));
            }
            else if (leftType == VarState.Types.String)
            {
                andExpressionFunction.statements.lines.Add(new IndentedLine
                ("if (var_0.Length != 0)", 0));
            }
            else
            {
                andExpressionFunction.statements.lines.Add(new IndentedLine
                ("if (var_0)", 0));
            }
            andExpressionFunction.statements.lines.Add(new IndentedLine
                ("{", 1));
            andExpressionFunction.statements.lines.Add(new IndentedLine
                ("return " + rightVisitor.result.ToString() + ";", -1));
            
            andExpressionFunction.statements.lines.Add(new IndentedLine
                ("}", 0));
            andExpressionFunction.statements.lines.Add(new IndentedLine
                ("return var_0;", 0));
            Function parentFunction = state.output.currentClasses.Peek().currentFunctions.Peek();
            parentFunction.pendingGeneratedFunctionsInScope.Add(andExpressionFunction);
            andExpressionFunction.parentClass = state.output.currentClasses.Peek();
            result.tokens.Add("GeneratedAndExpression" + num + "()");

            result.tokens.Add(")");
        }
        else
        {
            // Expression is standalone:
            if (!state.stmtState.isLocked)
            {
                state.stmtState.isStandalone = true;
                state.stmtState.isLocked = true;
            }
            int n = context.ChildCount;
            int i = 0;
            while (i < n)
            {
                if (i != 0)
                {
                    result.tokens.Add("&&");
                }
                NotTestVisitor newVisitor = new NotTestVisitor(state);

                // Explicitly convert to boolean.
                result.tokens.Add("Convert.ToBoolean(");

                context.GetChild(i).Accept(newVisitor);
                for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
                {
                    result.tokens.Add(newVisitor.result.tokens[j]);
                }
                result.tokens.Add(")");
                i += 2;
            }
        }
        return result;
    }



}