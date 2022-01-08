public class State
{
    public VarState varState;
    public StmtState stmtState;
    public LoopState loopState;
    public FuncCallState funcCallState;
    public Output output;
    public State()
    {
        varState = new VarState();
        stmtState = new StmtState();
        loopState = new LoopState();
        funcCallState = new FuncCallState();
        output = new Output();
    }
}