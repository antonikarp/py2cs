public class State
{
    public VarState varState;
    public StmtState stmtState;
    public LoopState loopState;
    public Output output;
    public State()
    {
        varState = new VarState();
        stmtState = new StmtState();
        loopState = new LoopState();
        output = new Output();
    }
}