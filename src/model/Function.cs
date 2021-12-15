using System.Text;
using System.Collections.Generic;

// This class is a model for a function.
public class Function
{
    public BlockModel statements;
    public string name;
    public bool isVoid;
    public Function()
    {
        isVoid = true;
        statements = new BlockModel();
    }
}