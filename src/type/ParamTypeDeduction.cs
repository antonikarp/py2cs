public class ParamTypeDeduction
{
    // This is a simple type deducer for types int, double, string.
    // By default the type is int.
    // If we encounter a dot then the type is overriden to double.
    // If we encounter a quotation mark them the type is overriden to string.
    public static VarState.Types Deduce(string value)
    {
        VarState.Types result = VarState.Types.Int;
        for (int i = 0; i < value.Length; ++i)
        {
            if (value[i] == '.')
            {
                result = VarState.Types.Double;
            }
        }
        for (int i = 0; i < value.Length; ++i)
        {
            if (value[i] == '\"')
            {
                result = VarState.Types.String;
            }
        }
        return result;
    }
}