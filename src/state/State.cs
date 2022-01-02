public class State
{
    public ClassState classState;
    public FuncState funcState;
    public VarState varState;
    public StmtState stmtState;
    public Output output;
    public State()
    {
        classState = new ClassState();
        funcState = new FuncState();
        varState = new VarState();
        stmtState = new StmtState();
        output = new Output();
    }
}