public class VarState
{
    public enum Types { List, Dictionary, HashSet, Other }
    public Types type;
    public VarState()
    {
        type = Types.Other;
    }
}