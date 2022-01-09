public class State
{
    public VarState varState;
    public StmtState stmtState;
    public LoopState loopState;
    public FuncCallState funcCallState;
    public CompForState compForState;
    public LhsTupleState lhsTupleState;
    public Output output;
    public State()
    {
        varState = new VarState();
        stmtState = new StmtState();
        loopState = new LoopState();
        funcCallState = new FuncCallState();
        compForState = new CompForState();
        lhsTupleState = new LhsTupleState();
        output = new Output();
    }
}