public class VarState
{
    public enum Types { List, Dictionary, HashSet, Other, Tuple, Int, Double, String }
    public Types type;
    public VarState()
    {
        type = Types.Other;
    }
}