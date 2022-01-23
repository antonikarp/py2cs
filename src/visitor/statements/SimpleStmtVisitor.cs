using Antlr4.Runtime.Misc;
// This is a visitor used for 'simple expressions' where sub-expressions are
// seperated by a semicolon.
public class SimpleStmtVisitor : Python3ParserBaseVisitor<BlockModel>
{
    public BlockModel result;
    public State state;
    public SimpleStmtVisitor(State _state)
    {
        state = _state;
    }
    public override BlockModel VisitSimple_stmt([NotNull] Python3Parser.Simple_stmtContext context)
    {
        // simple_stmt: small_stmt (';' small_stmt)* (';')? NEWLINE;
        result = new BlockModel();
        int n = context.ChildCount;
        int i = 0;
        while (i < n)
        {
            if (context.GetChild(i).ToString() != ";")
            {
                SmallStmtVisitor newVisitor = new SmallStmtVisitor(state);
                context.GetChild(i).Accept(newVisitor);
                IndentedLine newLine = new IndentedLine(newVisitor.result.ToString() + ";", 0);
                result.lines.Add(newLine);
            }
            i += 2;
        }

        return result;
    }


}