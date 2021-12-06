using Antlr4.Runtime.Misc;

public class CompOpVisitor : Python3ParserBaseVisitor<CompOp>
{
    public CompOp result;
    public override CompOp VisitComp_op([NotNull] Python3Parser.Comp_opContext context)
    {
        result = new CompOp();
        result.value = context.GetText();
        return result;
    }
}