public class State
{
    public VarState varState;
    public StmtState stmtState;
    public Output output;
    public State()
    {
        varState = new VarState();
        stmtState = new StmtState();
        output = new Output();
    }
}