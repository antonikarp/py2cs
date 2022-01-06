public class VarState
{
    public enum Types { List, Dictionary, HashSet, Other, Tuple, Int, Double, String, StringArray, ListComp }
    public Types type;
    public VarState()
    {
        type = Types.Other;
    }
}