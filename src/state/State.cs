public class State
{
    public ClassState classState;
    public FuncState funcState;
    public VarState varState;
    public State()
    {
        classState = new ClassState();
        funcState = new FuncState();
        varState = new VarState();
    }
}