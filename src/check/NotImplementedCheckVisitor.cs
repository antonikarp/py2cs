using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

public class NotImplementedCheckVisitor : Python3ParserBaseVisitor<Empty>
{
    public Empty empty;
    public NotImplementedModel model;
    public List<string> notImplementedAttributes; 
    public NotImplementedCheckVisitor()
    {
        model = new NotImplementedModel();
        notImplementedAttributes = new List<string> { "__class__", "__bases__" };
    }

    // Starred expressions.
    public override Empty VisitStar_expr([NotNull] Python3Parser.Star_exprContext context)
    {
        throw new NotImplementedException("Starred expressions.", context.Start.Line);
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
            throw new NotImplementedException("Multiple inheritance.", context.Start.Line);
        }
        // We do not accept a redeclaration of a class.
        if (model.declaredClasses.Contains(context.GetChild(1).ToString()))
        {
            throw new NotImplementedException("Redeclaration of a class.", context.Start.Line);
        }
        else
        {
            model.declaredClasses.Add(context.GetChild(1).ToString());
        }

        return VisitChildren(context);
    }

    // Async statements.
    public override Empty VisitAsync_stmt([NotNull] Python3Parser.Async_stmtContext context)
    {
        throw new NotImplementedException("Async statement.", context.Start.Line);
    }

    // Del statements.
    public override Empty VisitDel_stmt([NotNull] Python3Parser.Del_stmtContext context)
    {
        throw new NotImplementedException("Del statement.", context.Start.Line);
    }

    // Statements like 'from lib import foo'
    public override Empty VisitImport_from([NotNull] Python3Parser.Import_fromContext context)
    {
        throw new NotImplementedException("A 'from' import statement.", context.Start.Line);
    }

    // Function definitions - save the name of the function
    public override Empty VisitFuncdef([NotNull] Python3Parser.FuncdefContext context)
    {
        model.declaredFunctions.Add(context.NAME().ToString());
        return VisitChildren(context);
    }

    // Atom expr - check for function attributes whose expressions have the form:
    // 'f.x' where 'f' is a previously declared function.
    public override Empty VisitAtom_expr([NotNull] Python3Parser.Atom_exprContext context)
    {
        // In case of function attributes we have the following children:
        // Take for instance 'f.x'
        // Child #0 - atom    - Child #0: name('f')
        // Child #1 - trailer - Child #0: "."
        // ...                  ...

        if (context.ChildCount >= 2 &&
            context.GetChild(0).ChildCount >= 1 &&
            model.declaredFunctions.Contains(context.GetChild(0).GetChild(0).ToString()) &&
            context.GetChild(1).ChildCount >= 1 &&
            context.GetChild(1).GetChild(0).ToString() == ".")
        {
            throw new NotImplementedException("Function attributes.", context.Start.Line);
        }
        // Tuple comprehension is not handled.
        else if (context.atom() != null && context.atom().NAME() != null &&
            context.atom().NAME().ToString() == "tuple")
        {
            throw new NotImplementedException("Tuple comprehension.", context.Start.Line);
        }


        return VisitChildren(context);
    }

    public override Empty VisitTrailer([NotNull] Python3Parser.TrailerContext context)
    {
        // Attributes __class__ and __bases__ are not implemented
        // Take for instace 'c.__class__'
        // trailer - Child #0: "."
        //           Child #1: __class__
        if (context.NAME() != null && notImplementedAttributes.Contains(context.NAME().ToString()))
        {
            throw new NotImplementedException("Class attribute: " + context.NAME().ToString(), context.Start.Line);
        }
        return VisitChildren(context);
    }

    // We do not handle lambdas.
    public override Empty VisitLambdef([NotNull] Python3Parser.LambdefContext context)
    {
        throw new NotImplementedException("Lambda expression.", context.Start.Line);
    }

    // We do not handle 'with' statements.
    public override Empty VisitWith_stmt([NotNull] Python3Parser.With_stmtContext context)
    {
        throw new NotImplementedException("A 'with' statement.", context.Start.Line); 
    }

    // Ellipsis (...) is not handled.
    public override Empty VisitAtom([NotNull] Python3Parser.AtomContext context)
    {
        if (context.ELLIPSIS() != null)
        {
            throw new NotImplementedException("Ellipsis.", context.Start.Line);
        }
        return VisitChildren(context);
    }

    // Decorators are not handled.
    public override Empty VisitDecorated([NotNull] Python3Parser.DecoratedContext context)
    {
        throw new NotImplementedException("Decorators.", context.Start.Line);
    }

    // Try-else block is not handled.
    public override Empty VisitTry_stmt([NotNull] Python3Parser.Try_stmtContext context)
    {
        if (context.ELSE() != null)
        {
            throw new NotImplementedException("Try-else block.", context.Start.Line); ;
        }
        return VisitChildren(context);
    }
}