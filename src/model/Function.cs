using System.Text;
using System.Collections.Generic;

// This class is a model for a function.
public class Function
{
    public BlockModel statements;
    public string name;
    public bool isVoid;
    public bool isStatic;
    public bool isEnumerable;
    public List<string> parameters;
    public Dictionary<string, string> defaultParameters;
    public Dictionary<string, VarState.Types> defaultParameterTypes;
    public Function()
    {
        // By default this value is true, however when the visitor encounters
        // a return statement with expression, it becomes false.
        isVoid = true;
        // For now functions are static, so that they can be called from
        // the static Main method in class Program.
        isStatic = true;
        // After encountering a yield expression the return type becomes:
        // IEnumerable<dynamic>
        isEnumerable = false;
        statements = new BlockModel();
        parameters = new List<string>();
        defaultParameterTypes = new Dictionary<string, VarState.Types>();
}
}