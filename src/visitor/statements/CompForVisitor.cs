using System;
using Antlr4.Runtime.Misc;

// This is a visitor to be used to compute an expression in list comprehensions.
public class CompForVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public bool visited;
    public CompForVisitor(State _state)
    {
        state = _state;
        visited = false;
    }
    public override LineModel VisitComp_for([NotNull] Python3Parser.Comp_forContext context)
    {
        visited = true;
        result = new LineModel();

        // In case of no "if" clause, we have the following children
        // Child #0: for
        // Child #1: exprlist (for now assume that it is a single expr)
        // Child #2: in
        // Child #3: or_test

        // We will use Linq to translate it into C#:
        // [expr_x for x in y] => (from x in y select expr_x).ToList();

        // In case of "if" clause, we have the following children:
        // Child #0: for
        // Child #1: exprlist (for now assume that it is a single expr)
        // Child #2: in
        // Child #3: or_test
        // Child #4: comp_iter
        //
        // This translates to:
        // [expr_x for x in y if expr_x_2] => (from x in y where expr_x_2 select expr_x).ToList();
        if (context.ChildCount >= 4)
        {
            // Override the trype to be ListComp
            state.varState.type = VarState.Types.ListComp;

            result.tokens.Add("(from ");
            ExprVisitor iteratorVisitor = new ExprVisitor(state);
            context.GetChild(1).Accept(iteratorVisitor);
            for (int i = 0; i < iteratorVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(iteratorVisitor.result.tokens[i]);
            }
            result.tokens.Add(" in ");
            
            OrTestVisitor collectionVisitor = new OrTestVisitor(state);
            context.GetChild(3).Accept(collectionVisitor);
            for (int i = 0; i < collectionVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(collectionVisitor.result.tokens[i]);
            }

            // "If" clause present. 
            if (context.ChildCount == 5)
            {
                CompIfVisitor ifVisitor = new CompIfVisitor(state);
                context.GetChild(4).Accept(ifVisitor);
                result.tokens.Add(" where ");
                for (int i = 0; i < ifVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(ifVisitor.result.tokens[i]);
                }
            }
        }
        return result;
    }
}