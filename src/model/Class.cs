using System.Text;
// This class is a model for a class.
public class Class
{
    public string name;
    public Function mainMethod;
    public OutputBuilder outputBuilder;
    public Class(OutputBuilder _outputBuilder)
    {
        mainMethod = new Function();
        outputBuilder = _outputBuilder;
    }

}