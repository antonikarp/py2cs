using Antlr4.Runtime.Misc;

public class NotImplementedCheckVisitor : Python3ParserBaseVisitor<Empty>
{
    public Empty empty;
    public bool isNotImplemented;
    public NotImplementedCheckVisitor()
    {
        isNotImplemented = false;
    }
    // Starred expressions.
    public override Empty VisitStar_expr([NotNull] Python3Parser.Star_exprContext context)
    {
        isNotImplemented = true;
        return VisitChildren(context);
    }
    // Multiple inheritance.
    public override Empty VisitClassdef([NotNull] Python3Parser.ClassdefContext context)
    {
        // In a class defintion with multiple inheritance we have the following children:
        // Child #0: "class"
        // Child #1: <name>
        // Child #2: "("
        // Child #3: arglist <- has at least 3 children: #0: class_name_1
        //                                               #1: ","
        //                                               #2: class_name_2
        // Child #4: ")"
        // ...

        if (context.ChildCount >= 5 && context.GetChild(2).ToString() == "(" &&
            context.GetChild(4).ToString() == ")" &&
            context.GetChild(3).GetType().ToString() == "Python3Parser+ArglistContext" &&
            context.GetChild(3).ChildCount >= 3)
        {
            isNotImplemented = true;
        }
        return VisitChildren(context);
    }
}