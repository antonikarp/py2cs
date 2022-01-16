using System;
using System.Text;
using Antlr4.Runtime.Misc;
public class FlowStmtVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public FlowStmtVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitFlow_stmt([NotNull] Python3Parser.Flow_stmtContext context)
    {
        result = new LineModel();
        if (context.break_stmt() != null)
        {
            result.tokens.Add("break");
        }
        else if (context.continue_stmt() != null)
        {
            result.tokens.Add("continue");
        }
        else if (context.return_stmt() != null)
        {
            result.tokens.Add("return");
            // We have a case: "return expr";
            if (context.return_stmt().ChildCount == 2)
            {
                // This is not a standalone expression.
                state.stmtState.isStandalone = false;
                state.stmtState.isLocked = true;

                state.output.currentClasses.Peek().currentFunctions.Peek().isVoid = false;
                TestVisitor newVisitor = new TestVisitor(state);
                context.return_stmt().GetChild(1).Accept(newVisitor);
                result.tokens.Add(" ");
                for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
                {
                    result.tokens.Add(newVisitor.result.tokens[i]);
                }

                // Check if we return a function, if so - override return type
                foreach (var function in state.output.currentClasses.Peek().currentFunctions.Peek().internalFunctions)
                {
                    if (newVisitor.result.ToString() == function.name)
                    {
                        state.output.currentClasses.Peek().currentFunctions.Peek().overridenReturnType = function.getDelegateType();
                    }
                }
            }
        }
        else if (context.yield_stmt() != null)
        {
            YieldExprVisitor newVisitor = new YieldExprVisitor(state);
            context.GetChild(0).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        return result;
    }


}