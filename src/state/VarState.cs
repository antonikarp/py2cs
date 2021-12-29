public class VarState
{
    public enum Types { List, Dictionary, HashSet, Other, Tuple }
    public Types type;
    public VarState()
    {
        type = Types.Other;
    }
}