using System.Text;
using System.Collections.Generic;

// This class is a model for the entire program.
// For now, it translates expressions to the Main method in the Program class.
public class Output
{
    public State state;
    public List<IndentedLine> internalLines;
    public int indentationLevel = 0;

    public Output()
    {
        state = new State();
        internalLines = new List<IndentedLine>();
    }

    public string getIndentedLine(string str)
    {
        string result = "";
        for (int i = 0; i < indentationLevel; ++i)
        {
            result += "    ";
        }
        result += str;
        return result;
    }


    public override string ToString()
    {
        OutputBuilder outputBuilder = new OutputBuilder();
        foreach (var dir in state.classState.usingDirs)
        {
            outputBuilder.commitIndentedLine(new IndentedLine("using " + dir + ";", 0));
        }
        outputBuilder.commitIndentedLine(new IndentedLine("class Program", 0));
        outputBuilder.commitIndentedLine(new IndentedLine("{", 1));

        foreach (var func in state.classState.functions)
        {
            string firstLine = "public ";
            if (func.isStatic)
            {
                firstLine += "static ";
            }

            if (func.isVoid)
            {
                firstLine += "void ";
            }
            else if (!func.isVoid && !func.isEnumerable)
            {
                firstLine += "dynamic ";
            }
            else if (!func.isVoid && func.isEnumerable)
            {
                firstLine += "IEnumerable<dynamic> ";
            }

            firstLine += func.name;
            firstLine += "(";
            for (int i = 0; i < func.parameters.Count; ++i)
            {
                if (i != 0)
                {
                    firstLine += ", ";
                }
                
                // Case of a default parameter.
                if (func.defaultParameters.ContainsKey(func.parameters[i]) &&
                    func.defaultParameterTypes.ContainsKey(func.parameters[i]))
                {
                    switch (func.defaultParameterTypes[func.parameters[i]])
                    {
                        case VarState.Types.Int:
                            firstLine += "int ";
                            break;
                        case VarState.Types.Double:
                            firstLine += "double ";
                            break;
                        case VarState.Types.String:
                            firstLine += "string ";
                            break;
                        default:
                            firstLine += "dynamic ";
                            break;
                    }
                    firstLine += func.parameters[i];
                    firstLine += " = ";
                    firstLine += func.defaultParameters[func.parameters[i]];
                }
                // Case of a positional (usual) parameter
                else
                {
                    firstLine += "dynamic ";
                    firstLine += func.parameters[i];
                }
            }
            firstLine += ")";

            outputBuilder.commitIndentedLine(new IndentedLine(firstLine, 0));
            outputBuilder.commitIndentedLine(new IndentedLine("{", 1));
            foreach (var indentedLine in func.statements.lines)
            {
                outputBuilder.commitIndentedLine(indentedLine);
            }
            outputBuilder.commitIndentedLine(new IndentedLine("", -1));
            outputBuilder.commitIndentedLine(new IndentedLine("}", 0));
        }

        if (internalLines.Count > 0)
        {
            outputBuilder.commitIndentedLine(new IndentedLine("static void Main(string[] args)", 0));
            outputBuilder.commitIndentedLine(new IndentedLine("{", 1));
            foreach (var indentedLine in internalLines)
            {
                outputBuilder.commitIndentedLine(indentedLine);
            }
            outputBuilder.commitIndentedLine(new IndentedLine("", -1));
            outputBuilder.commitIndentedLine(new IndentedLine("}", 0));
        }
        else if (internalLines.Count == 0)
        // Empty entry point
        {
            outputBuilder.commitIndentedLine(new IndentedLine("static void Main(string[] args)", 0));
            outputBuilder.commitIndentedLine(new IndentedLine("{", 0));
            outputBuilder.commitIndentedLine(new IndentedLine("}", 0));
        }
        outputBuilder.commitIndentedLine(new IndentedLine("", -1));
        outputBuilder.commitIndentedLine(new IndentedLine("}", 0));
        return outputBuilder.output.ToString();
    }
}
