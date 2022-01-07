public class State
{
    public VarState varState;
    public StmtState stmtState;
    public ForStmtState forStmtState;
    public Output output;
    public State()
    {
        varState = new VarState();
        stmtState = new StmtState();
        forStmtState = new ForStmtState();
        output = new Output();
    }
}