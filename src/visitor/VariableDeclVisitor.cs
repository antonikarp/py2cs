using Antlr4.Runtime.Misc;
// This is a visitor f
public class VariableDeclVisitor : Python3ParserBaseVisitor<VariableDecl>
{
    public VariableDecl result;
    public override VariableDecl VisitExpr_stmt([NotNull] Python3Parser.Expr_stmtContext context)
    {
        result = new VariableDecl();
        // We assume that this node has exactly three children in the parse tree.
        // * The first child contains the variable name.
        // * The second child is the "=" operator.
        // * The third child contains the expression which evaluates into the initial value
        //   of the variable 
        VariableDeclVisitor newVisitor = new VariableDeclVisitor();
        context.GetChild(0).Accept(newVisitor);
        result.name = newVisitor.result.name;
        ArithExprVisitor arithVisitor = new ArithExprVisitor();
        context.GetChild(2).Accept(arithVisitor);
        result.value = arithVisitor.result;
        return result;
    }
    // A method for obtaining the variable name.
    public override VariableDecl VisitAtom_expr([NotNull] Python3Parser.Atom_exprContext context)
    {
        result = new VariableDecl();
        if (context.atom().NAME() != null)
        {
            result.name = context.atom().NAME().ToString();
        }
        return result;
    }

}