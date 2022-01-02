﻿using System.Text;
using System.Collections.Generic;

// This class is a model for a function.
public class Function
{
    public BlockModel statements;
    public string name;
    public bool isVoid;
    public bool isStatic;
    public bool isPublic;
    public bool isEnumerable;
    public List<string> parameters;
    public Dictionary<string, string> defaultParameters;
    public Dictionary<string, VarState.Types> defaultParameterTypes;
    public OutputBuilder outputBuilder;
    public Dictionary<string, VarState.Types> variables;
    public List<Function> internalFunctions;
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

        // By default the function is public, it is changed in internal functions.
        isPublic = true;

        statements = new BlockModel();
        parameters = new List<string>();
        defaultParameters = new Dictionary<string, string>();
        defaultParameterTypes = new Dictionary<string, VarState.Types>();
        internalFunctions = new List<Function>();
        variables = new Dictionary<string, VarState.Types>();
    }
    public void CommitToOutput()
    {
        string firstLine = "";
        if (isPublic)
        {
            firstLine += "public ";
        }
        if (isStatic)
        {
            firstLine += "static ";
        }

        if (isVoid)
        {
            firstLine += "void ";
        }
        else if (!isVoid && !isEnumerable)
        {
            firstLine += "dynamic ";
        }
        else if (!isVoid && isEnumerable)
        {
            firstLine += "IEnumerable<dynamic> ";
        }

        firstLine += name;
        firstLine += "(";
        for (int i = 0; i < parameters.Count; ++i)
        {
            if (i != 0)
            {
                firstLine += ", ";
            }

            // Case of a default parameter.
            if (defaultParameters.ContainsKey(parameters[i]) &&
                defaultParameterTypes.ContainsKey(parameters[i]))
            {
                switch (defaultParameterTypes[parameters[i]])
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
                firstLine += parameters[i];
                firstLine += " = ";
                firstLine += defaultParameters[parameters[i]];
            }
            // Case of a positional (usual) parameter
            else
            {
                firstLine += "dynamic ";
                firstLine += parameters[i];
            }
        }
        firstLine += ")";

        outputBuilder.commitIndentedLine(new IndentedLine(firstLine, 0));
        outputBuilder.commitIndentedLine(new IndentedLine("{", 1));
        // Commit each internal function.
        foreach (var internalFunc in internalFunctions)
        {
            // Internal functions are not public.
            internalFunc.isPublic = false;
            internalFunc.outputBuilder = outputBuilder;
            internalFunc.CommitToOutput();
        }

        foreach (var indentedLine in statements.lines)
        {
            outputBuilder.commitIndentedLine(indentedLine);
        }
        outputBuilder.commitIndentedLine(new IndentedLine("", -1));
        outputBuilder.commitIndentedLine(new IndentedLine("}", 0));
    }
}