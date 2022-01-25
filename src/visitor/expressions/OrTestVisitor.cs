using System;
using Antlr4.Runtime.Misc;

// This is a visitor used to compute an expression created with a logical operator "or".
// It traverses the parse tree from the node "or_test".

public class OrTestVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public OrTestVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitOr_test([NotNull] Python3Parser.Or_testContext context)
    {
        result = new LineModel();
        // If there is one child then it is a 'and_test' node.
        if (context.ChildCount == 1)
        {
            AndTestVisitor newVisitor = new AndTestVisitor(state);
            context.GetChild(0).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        // If there are more than one child then we have the following children:
        // Child #0: <expr1>
        // Child #1: or
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
            AndTestVisitor leftVisitor = new AndTestVisitor(state);
            context.GetChild(0).Accept(leftVisitor);
            AndTestVisitor rightVisitor = new AndTestVisitor(state);
            context.GetChild(2).Accept(rightVisitor);
            VarState.Types leftType = ParamTypeDeduction.Deduce(leftVisitor.result.ToString());

            // a or b is translated into
            //
            // var var_0 = a;
            // if (func(var_0))
            // {
            //      return var_0;
            // }
            // else
            // {
            //      return b;
            // }
            // where func is a type specific bool converter:
            // for list: .Count > 0, for int: != 0

            ++state.output.currentClasses.Peek().currentGeneratedOrExpressionNumber;
            var num = state.output.currentClasses.Peek().currentGeneratedOrExpressionNumber;
            Function orExpressionFunction = new Function(state.output);
            orExpressionFunction.name = "GeneratedOrExpression" + num;
            orExpressionFunction.isPublic = false;
            orExpressionFunction.isVoid = false;
            orExpressionFunction.isStatic = false;
            orExpressionFunction.statements.lines.Add(new IndentedLine
                ("var var_0 = " + leftVisitor.result.ToString() + ";", 0));
            if (leftType == VarState.Types.Int || leftType == VarState.Types.Double)
            {
                orExpressionFunction.statements.lines.Add(new IndentedLine
                ("if (var_0 != 0)", 0));
            }
            else if (leftType == VarState.Types.String)
            {
                orExpressionFunction.statements.lines.Add(new IndentedLine
                ("if (var_0.Length != 0)", 0));
            }
            else
            {
                orExpressionFunction.statements.lines.Add(new IndentedLine
                ("if (var_0)", 0));
            }
            orExpressionFunction.statements.lines.Add(new IndentedLine
                ("{", 1));
            orExpressionFunction.statements.lines.Add(new IndentedLine
                ("return var_0;", -1));
            orExpressionFunction.statements.lines.Add(new IndentedLine
                ("}", 0));
            orExpressionFunction.statements.lines.Add(new IndentedLine
                ("return " + rightVisitor.result.ToString() + ";", 0));
            Function parentFunction = state.output.currentClasses.Peek().currentFunctions.Peek();
            parentFunction.pendingGeneratedFunctionsInScope.Add(orExpressionFunction);
            orExpressionFunction.parentClass = state.output.currentClasses.Peek();
            result.tokens.Add("GeneratedOrExpression" + num + "()");
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
                    result.tokens.Add("||");
                }
                AndTestVisitor newVisitor = new AndTestVisitor(state);
                context.GetChild(i).Accept(newVisitor);

                // Explicitly convert to boolean.
                result.tokens.Add("Convert.ToBoolean(");

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