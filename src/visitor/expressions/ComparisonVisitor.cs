﻿using System;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;

// This is a visitor used to compute a "shift" expression composed with operators
// "<<" and ">>". It traverses the parse tree from the node "shift_expr".
public class ComparisonVisitor : Python3ParserBaseVisitor<Comparison>
{
    public Comparison result;
    public override Comparison VisitComparison([NotNull] Python3Parser.ComparisonContext context)
    {
        result = new Comparison();
        // If there is one child then it is shift expression.
        if (context.ChildCount == 1)
        {
            ShiftExprVisitor newVisitor = new ShiftExprVisitor();
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
            ShiftExprVisitor leftVisitor = new ShiftExprVisitor();
            ShiftExprVisitor rightVisitor = new ShiftExprVisitor();
            context.GetChild(0).Accept(leftVisitor);
            context.GetChild(2).Accept(rightVisitor);
            for (int i = 0; i < leftVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(leftVisitor.result.tokens[i]);
            }
            CompOpVisitor opVisitor = new CompOpVisitor();
            context.GetChild(1).Accept(opVisitor);
            result.tokens.Add(opVisitor.result.value);
            for (int i = 0; i < rightVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(rightVisitor.result.tokens[i]);
            }
        }
        // If there are more than 3 children then we have a chained comparison
        // For instance: a < b < c
        else
        {
            int numberOfExpr = context.ChildCount / 2 + 1;
            List<ShiftExprVisitor> visitors = new List<ShiftExprVisitor>();
            List<CompOpVisitor> opVisitors = new List<CompOpVisitor>();
            // Invoke visitors on all of the child expressions.
            for (int i = 0; i < numberOfExpr; ++i)
            {
                ShiftExprVisitor newVisitor = new ShiftExprVisitor();
                context.GetChild(i * 2).Accept(newVisitor);
                visitors.Add(newVisitor);
                if (i != numberOfExpr - 1)
                {
                    CompOpVisitor newOpVisitor = new CompOpVisitor();
                    context.GetChild(i * 2 + 1).Accept(newOpVisitor);
                    opVisitors.Add(newOpVisitor);
                }
            }
            // Build the conjuction of pairs of statements. For instance:
            // a < b < c -> (a < b) && (b < c)
            for (int i = 0; i < numberOfExpr - 1; ++i)
            {
                result.tokens.Add("(");
                for (int j = 0; j < visitors[i].result.tokens.Count; ++j)
                {
                    result.tokens.Add(visitors[i].result.tokens[j]);
                }
                result.tokens.Add(opVisitors[i].result.value);
                for (int j = 0; j < visitors[i + 1].result.tokens.Count; ++j)
                {
                    result.tokens.Add(visitors[i + 1].result.tokens[j]);
                }
                result.tokens.Add(")");
                if (i != numberOfExpr - 2)
                {
                    result.tokens.Add("&&");
                }
            }
        }
        return result;
    }

}