using Antlr4.Runtime.Misc;

public class TestListCompVisitor : Python3ParserBaseVisitor<TestListComp>
{
    public TestListComp result;
    public ClassState classState;
    public TestListCompVisitor(ClassState _classState)
    {
        classState = _classState;
    }
    public override TestListComp VisitTestlist_comp([NotNull] Python3Parser.Testlist_compContext context)
    {
        result = new TestListComp();
        for (int i = 0; i < context.ChildCount; ++i)
        {
            if (context.GetChild(i).ToString() == ",")
            {
                result.tokens.Add(", ");
            }
            else
            {
                OrTestVisitor newVisitor = new OrTestVisitor(classState);
                context.GetChild(i).Accept(newVisitor);
                for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
                {
                    result.tokens.Add(newVisitor.result.tokens[j]);
                }
            }
        }
        return result;
    }

}