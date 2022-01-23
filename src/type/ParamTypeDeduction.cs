using System;
public class ParamTypeDeduction
{
    // This is a simple type deducer for types int, double, string.
    // By default the type is int.
    // If we encounter a dot then the type is overriden to double.
    // If we encounter a quotation mark them the type is overriden to string.
    public static VarState.Types Deduce(string value)
    {
        int varInt;
        double varDouble;

        if (int.TryParse(value, out varInt))
        {
            return VarState.Types.Int;
        }
        if (double.TryParse(value, out varDouble))
        {
            return VarState.Types.Double;
        }
        if (value.Length > 0 && (value[0] == '\"' || value[0] == '\''))
        {
            return VarState.Types.String;
        }
        if (value.StartsWith("new List"))
        {
            return VarState.Types.List;
        }
        return VarState.Types.Other;
    }
}