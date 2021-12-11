using Antlr4.Runtime.Misc;

public class FactorVisitor : Python3ParserBaseVisitor<Factor>
{
    public Factor result;
    public ClassState classState;
    public FactorVisitor(ClassState _classState)
    {
        classState = _classState;
    }
    public override Factor VisitFactor([NotNull] Python3Parser.FactorContext context)
    {
        result = new Factor();
        
        // Case of the unary '+' or '-'
        if (context.ChildCount == 2)
        {
            result.tokens.Add(context.GetChild(0).ToString());
            FactorVisitor newVisitor = new FactorVisitor(classState);
            context.GetChild(1).Accept(newVisitor);
            for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
            {
                result.tokens.Add(newVisitor.result.tokens[j]);
            }
        }
        else
        {
            AtomExprVisitor newVisitor = new AtomExprVisitor(classState);
            context.GetChild(0).Accept(newVisitor);
            for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
            {
                result.tokens.Add(newVisitor.result.tokens[j]);
            }
        }
        
        return result;

    }

}