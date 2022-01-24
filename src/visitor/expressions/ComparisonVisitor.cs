using System;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;

// This is a visitor used to compute a "shift" expression composed with operators
// "<<" and ">>". It traverses the parse tree from the node "shift_expr".
public class ComparisonVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public ComparisonVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitComparison([NotNull] Python3Parser.ComparisonContext context)
    {
        result = new LineModel();
        // If there is one child then it is a "expr".
        if (context.ChildCount == 1)
        {
            ExprVisitor newVisitor = new ExprVisitor(state);
            context.GetChild(0).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        // If there are 3 children then we have 2 expressions joined by
        // a comparison operator (<, >, <=, >=, ==, !=)
        else if (context.ChildCount == 3)
        {
            // Expression is standalone:
            if (!state.stmtState.isLocked)
            {
                state.stmtState.isStandalone = true;
                state.stmtState.isLocked = true;
            }
            CompOpVisitor opVisitor = new CompOpVisitor(state);
            ExprVisitor leftVisitor = new ExprVisitor(state);
            ExprVisitor rightVisitor = new ExprVisitor(state);
            context.GetChild(0).Accept(leftVisitor);
            context.GetChild(1).Accept(opVisitor);
            context.GetChild(2).Accept(rightVisitor);
            if (opVisitor.result.value == "==")
            {
                // list_1==list_2 -> list_1.SequenceEqual(list_2)

                string leftExpr = leftVisitor.result.ToString();
                string rightExpr = rightVisitor.result.ToString();
                if (state.output.currentClasses.Peek().currentFunctions.Peek().variables.ContainsKey(leftExpr) &&
                    state.output.currentClasses.Peek().currentFunctions.Peek().variables.ContainsKey(rightExpr))
                {
                    VarState.Types leftExprType = state.output.currentClasses.Peek().currentFunctions.Peek().variables[leftExpr];
                    VarState.Types rightExprType = state.output.currentClasses.Peek().currentFunctions.Peek().variables[leftExpr];
                    if (leftExprType == VarState.Types.List && rightExprType == VarState.Types.List)
                    {
                        // We use SequenceEqual from System.Linq
                        state.output.usingDirs.Add("System.Linq");

                        result.tokens.Add(leftExpr);
                        result.tokens.Add(".SequenceEqual(");
                        result.tokens.Add(rightExpr);
                        result.tokens.Add(")");
                        return result;
                    }
                }
            }
            if (opVisitor.result.value == "in")
            {
                // Membership test for a set.
                for (int i = 0; i < rightVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(rightVisitor.result.tokens[i]);
                }
                if (state.output.currentClasses.Peek().currentFunctions.Peek().variables.ContainsKey(rightVisitor.result.ToString()))
                {
                    VarState.Types type = state.output.currentClasses.Peek().currentFunctions.Peek().variables[rightVisitor.result.ToString()];
                    if (type == VarState.Types.Dictionary)
                    {
                        result.tokens.Add(".ContainsKey");
                    }
                    else if (type == VarState.Types.HashSet)
                    {
                        result.tokens.Add(".Contains");
                    }
                }
                else
                // This is the case where we construct a variable in place
                {
                    if (state.varState.type == VarState.Types.List)
                    {
                        result.tokens.Add(".Contains");
                    }
                }
                result.tokens.Add("(");
                for (int i = 0; i < leftVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(leftVisitor.result.tokens[i]);
                }
                result.tokens.Add(")");

                // Flush VarState
                state.varState = new VarState();
            }
            else if (opVisitor.result.value == "is")
            {
                // "is" operator - use IsOperator class from the Library.
                state.output.library.CommitIsOperator();
                result.tokens.Add("IsOperator.Compare(");
                for (int i = 0; i < leftVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(leftVisitor.result.tokens[i]);
                }
                result.tokens.Add(", ");
                for (int i = 0; i < rightVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(rightVisitor.result.tokens[i]);
                }
                result.tokens.Add(")");
            }
            else
            {
                for (int i = 0; i < leftVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(leftVisitor.result.tokens[i]);
                }
                result.tokens.Add(opVisitor.result.value);
                for (int i = 0; i < rightVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(rightVisitor.result.tokens[i]);
                }
            }

            
        }
        // If there are more than 3 children then we have a chained comparison
        // For instance: a < b < c
        else
        {
            int numberOfExpr = context.ChildCount / 2 + 1;
            List<ExprVisitor> visitors = new List<ExprVisitor>();
            List<CompOpVisitor> opVisitors = new List<CompOpVisitor>();
            // Invoke visitors on all of the child expressions.
            for (int i = 0; i < numberOfExpr; ++i)
            {
                ExprVisitor newVisitor = new ExprVisitor(state);
                context.GetChild(i * 2).Accept(newVisitor);
                visitors.Add(newVisitor);
                if (i != numberOfExpr - 1)
                {
                    CompOpVisitor newOpVisitor = new CompOpVisitor(state);
                    context.GetChild(i * 2 + 1).Accept(newOpVisitor);
                    opVisitors.Add(newOpVisitor);
                }
            }

            // a < b < c is translated into:
            //
            // var var_0 = a;
            // var var_1 = b;
            // if (!(var_0 < var_1))
            // {
            //   return false;
            // }
            // var var_2 = c;
            // if (!(var_1 < var_2))
            // {
            //   return false;
            // }
            // return true;

            ++state.output.currentClasses.Peek().currentGeneratedChainedComparisonNumber;
            var num = state.output.currentClasses.Peek().currentGeneratedChainedComparisonNumber;
            Function chainedComparisonFunction = new Function(state.output);
            chainedComparisonFunction.name = "GeneratedChainedComparison" + num;
            chainedComparisonFunction.isChainedComparison = true;
            chainedComparisonFunction.isPublic = false;
            for (int i = 0; i < numberOfExpr; ++i)
            {
                chainedComparisonFunction.statements.lines.Add(new IndentedLine
                    ("var var_" + i + " = " + visitors[i].result.ToString() + ";", 0));
                if (i != 0)
                {
                    chainedComparisonFunction.statements.lines.Add(new IndentedLine
                    ("if (!(var_" + (i - 1).ToString() + opVisitors[i - 1].result.value.ToString() +
                    "var_" + i + "))", 0));
                    chainedComparisonFunction.statements.lines.Add(new IndentedLine
                    ("{", 1));
                    chainedComparisonFunction.statements.lines.Add(new IndentedLine
                    ("return false;", -1));
                    chainedComparisonFunction.statements.lines.Add(new IndentedLine
                    ("}", 0));

                }
            }
            chainedComparisonFunction.statements.lines.Add(new IndentedLine
                ("return true;", 0));
            Function parentFunction = state.output.currentClasses.Peek().currentFunctions.Peek();
            parentFunction.internalFunctions.Add(chainedComparisonFunction);
            chainedComparisonFunction.parentClass = state.output.currentClasses.Peek();

            result.tokens.Add("GeneratedChainedComparison" + num + "()");

        }
        return result;
    }

}