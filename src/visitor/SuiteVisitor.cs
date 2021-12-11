using System;
using Antlr4.Runtime.Misc;

// This is a visitor to be used to compute a suite of statements
public class SuiteVisitor : Python3ParserBaseVisitor<Suite>
{
    public Suite result;
    public override Suite VisitSuite([NotNull] Python3Parser.SuiteContext context)
    {
        result = new Suite();
        for (int i = 0; i < context.ChildCount; ++i)
        {
            if (context.GetChild(i).GetType().ToString() == "Python3Parser+StmtContext")
             {
                // This means that the i-th child is a stmt node
                StmtVisitor newVisitor = new StmtVisitor();
                context.GetChild(i).Accept(newVisitor);
                for (int j = 0; j < newVisitor.result.lines.Count; ++j)
                {
                    result.lines.Add(newVisitor.result.lines[j]);
                }
            }

        }
        return result;
    }
}
