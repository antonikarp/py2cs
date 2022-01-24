public class VarState
{
    public enum Types { List, Dictionary, HashSet, Other, Tuple, Int, Double, String, StringArray, ListComp, ListInt, Overriden, ListFunc }
    public Types type;
    public string funcSignature;
    public VarState()
    {
        type = Types.Other;
        funcSignature = "";
    }
}