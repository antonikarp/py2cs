using System.Text;
using System.Collections.Generic;
// This class is a model for a class.
public class Class
{
    public string name;
    public Function mainMethod;
    public OutputBuilder outputBuilder;
    public Stack<Function> currentFunctions;
    public List<Function> functions;
    public Class(OutputBuilder _outputBuilder)
    {
        mainMethod = new Function();
        outputBuilder = _outputBuilder;
        currentFunctions = new Stack<Function>();
        functions = new List<Function>();
        // Function Main
        Function mainFunction = new Function();
        mainFunction.isVoid = true;
        mainFunction.isStatic = true;
        mainFunction.name = "Main";
        currentFunctions.Push(mainFunction);
    }

}