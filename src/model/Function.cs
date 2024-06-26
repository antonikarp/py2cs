﻿using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

// This class is a model for a function.
public class Function
{
    public BlockModel statements;
    public Class parentClass;
    public string name;
    public bool isVoid;
    public bool isStatic;
    public bool isPublic;
    public bool isConstructor;
    public bool isEnumerable;
    public List<string> parameters;
    public Dictionary<string, string> defaultParameters;
    public Dictionary<string, VarState.Types> defaultParameterTypes;
    public Dictionary<string, VarState.Types> variables;
    public List<Function> internalFunctions;
    public List<string> baseConstructorInitializerList;
    public List<List<VarState.Types>> usedParameterTypesInConstructor;
    public bool isChainedComparison;


    // Hidden identifiers at the function scope.
    public List<string> hiddenIdentifiers;

    // Overriden types - used when arguments are functions.
    public Dictionary<string, string> overridenParameterTypes;
    public string overridenReturnType;

    public Output output;
    // This indicates how many temporarary bool variables for entry to else blocks.
    public int currentGeneratedElseBlockEntryNumber = -1;

    // This set holds identifiers which refer to global variables.
    // They are prepended by class name like: Program.x
    public HashSet<string> identifiersReferringToGlobal;

    // This set hold identifiers which refer to nonlocal variables.
    // They won't be redeclared.
    public HashSet<string> identifiersReferringToNonlocal;

    // This list holds functions which should be added at the end of current scope.
    public List<Function> pendingGeneratedFunctionsInScope;

    // This maps the identifier of a tuple to its number of elements.
    // Example: a = (1, 2, 3) -> tupleIdentifierToNumberOfElements["a"] = 3
    public Dictionary<string, int> tupleIdentifierToNumberOfElements;

    // This maps the identifier of a list to a name of the class which is a type
    // of its elements. It is useful while iterating over such collection, because
    // if the iteration variable has type dynamic, it is impossible to
    // perform LINQ queries on such variable.
    public Dictionary<string, string> listIdentifiersToClassNames;

    // If an indentifier of a defined function was assigned to on the lhs,
    // we need to rename it.
    public HashSet<string> changedFunctionIdentifiers;

    public Function(Output _output)
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

        // Translated function __init__ is a constructor.
        isConstructor = false;

        // Represents a generated function for a chained comparison.
        isChainedComparison = false;

        statements = new BlockModel();
        parameters = new List<string>();
        defaultParameters = new Dictionary<string, string>();
        defaultParameterTypes = new Dictionary<string, VarState.Types>();
        internalFunctions = new List<Function>();
        variables = new Dictionary<string, VarState.Types>();
        baseConstructorInitializerList = new List<string>();
        parentClass = null;

        // Used parameter types in constructor
        usedParameterTypesInConstructor = new List<List<VarState.Types>>();

        overridenParameterTypes = new Dictionary<string, string>();
        overridenReturnType = "";
        hiddenIdentifiers = new List<string>();
        identifiersReferringToGlobal = new HashSet<string>();
        identifiersReferringToNonlocal = new HashSet<string>();
        pendingGeneratedFunctionsInScope = new List<Function>();
        tupleIdentifierToNumberOfElements = new Dictionary<string, int>();
        listIdentifiersToClassNames = new Dictionary<string, string>();
        changedFunctionIdentifiers = new HashSet<string>();

        output = _output;
    }
    public string getDelegateType()
    {
        string result = "Func<";
        for (int i = 0; i < parameters.Count; ++i)
        {
            if (i != 0)
            {
                result += ", ";
            }
            if (overridenParameterTypes.ContainsKey(parameters[i]))
            {
                result += overridenParameterTypes[parameters[i]];
            }
            else
            {
                result += "dynamic";
            }
        }
        // We always return a value - this is a return type:
        if (overridenReturnType != "")
        {
            result += ", ";
            result += overridenReturnType;
            result += ">";
        }
        else
        {
            result += ", dynamic>";
        }


        return result;
    }
    public void ReturnNullIfVoid()
    {
        if (isVoid && !isChainedComparison && !isConstructor && name != "Main")
        {
            IndentedLine newLine = new IndentedLine("return null;", 0);
            statements.lines.Add(newLine);
            isVoid = false;
        }
    }
    public void CheckForCollidingDeclarations()
    {
        Dictionary<string, int> seen = new Dictionary<string, int>();
        int curIndentation = 0;
        for (int i = 0; i < statements.lines.Count; ++i)
        {
            List<KeyValuePair<string, int>> itemsToRemove = new List<KeyValuePair<string, int>>();
            foreach (var kv in seen)
            {
                // We are outside of scope where kv.Key was defined (curIndentation is lower)
                // Remove these items from seen, so that they can be redeclared.
                if (kv.Value > curIndentation)
                {
                    itemsToRemove.Add(kv);
                }
            }
            foreach (var kv in itemsToRemove)
            {
                seen.Remove(kv.Key);
            }
            string line = statements.lines[i].line;
            string[] tokens = line.Split(" ");
            // There is no previous conflicting declaration present in 'seen'.
            // Add it to 'seen'. This declaration can stay.
            if (tokens[0] == "dynamic" && !seen.ContainsKey(tokens[1]))
            {
                seen.Add(tokens[1], curIndentation);
            }
            // We are inside of scope where this variable is defined (curIndentation is higher)
            // Change declaration to an assignment.
            else if (tokens[0] == "dynamic" && seen.ContainsKey(tokens[1])
                && curIndentation > seen[tokens[1]])
            {
                tokens[0] = "";
                string newLine = System.String.Join(" ", tokens);
                statements.lines[i].line = newLine;
            }
            // Update curIndentation.
            curIndentation += statements.lines[i].increment;
        }
    }
    public void ResolveGlobalVariables()
    {
        HashSet<string> staticFieldIdentifiers = parentClass.staticFieldIdentifiers;
        string className = parentClass.name;

        for (int i = 0; i < statements.lines.Count; ++i)
        {
            Regex rx = new Regex("@@@{(.*),(.*)}");
            MatchCollection matches = rx.Matches(statements.lines[i].line);
            foreach (Match match in matches)
            {
                if (!staticFieldIdentifiers.Contains(match.Groups[1].Value))
                {
                    throw new IncorrectInputException("Invalid identifier: '" + match.Groups[1].Value + "'. 'Global' statement is incorrect.", Int32.Parse(match.Groups[2].Value));
                }
                else
                {
                    string toReplace = "@@@{" + match.Groups[1].Value + "," + match.Groups[2].Value + "}";
                    string newLine = statements.lines[i].line;
                    newLine = newLine.Replace(toReplace, className + "." + match.Groups[1].Value);
                    statements.lines[i].line = newLine;
                }
            }
        }
    }
    public void ResolveNestedImports()
    {
        for (int i = 0; i < statements.lines.Count; ++i)
        {
            string oldLine = statements.lines[i].line;
            string newLine = oldLine;
            foreach (var kv in output.nestedImportNames)
            {
                newLine = newLine.Replace(kv.Key, kv.Value);
            }
            if (oldLine != newLine)
            {
                statements.lines[i].line = newLine;
            }
        }
    }

    public void CommitToOutput()
    {
        ReturnNullIfVoid();
        CheckForCollidingDeclarations();
        ResolveGlobalVariables();
        ResolveNestedImports();

        // Handle the default case (if this is not a constructor of the parent class
        // or even if it is not constructor) by storing a list of "dynamic" types.
        if (usedParameterTypesInConstructor.Count == 0)
        {
            List<VarState.Types> defaultTypes = new List<VarState.Types>();
            for (int i = 0; i < parameters.Count; ++i)
            {
                // For example when the parameter is a function.
                if (overridenParameterTypes.ContainsKey(parameters[i]))
                {
                    defaultTypes.Add(VarState.Types.Overriden);
                }
                defaultTypes.Add(VarState.Types.Other);
            }
            usedParameterTypesInConstructor.Add(defaultTypes);
        }
        foreach (var usedParameterTypes in usedParameterTypesInConstructor)
        {

            string firstLine = "";
            if (isConstructor)
            {
                firstLine += "public ";
            }
            else
            {
                if (isPublic)
                {
                    firstLine += "public ";
                }
                // For now, all methods outside of Main class are not static.
                if (isStatic && parentClass != null && parentClass.name == "Program")
                {
                    firstLine += "static ";
                }
                if (overridenReturnType != "")
                {
                    firstLine += overridenReturnType;
                    firstLine += " ";
                }
                else if (isChainedComparison)
                {
                    firstLine += "bool ";
                }
                else if (isVoid)
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
                    if (parentClass.name == "Program")
                    {
                        // We assign 'null' to the variable. The coalescing operation
                        // with the default parameter will be located at the beginning
                        // of the function.
                        firstLine += "dynamic ";
                        firstLine += parameters[i];
                        firstLine += " = ";
                        firstLine += "null";
                    }
                    else
                    {
                        // We assign a static type to the default parameter
                        // If we cannot recognize a type, then it is null, so
                        // mark it as 'dynamic'. This is an oversimplification but
                        // this tool lacks the proper type system.
                        switch (defaultParameterTypes[parameters[i]])
                        {

                            case VarState.Types.Double:
                                firstLine += "double ";
                                break;
                            case VarState.Types.String:
                                firstLine += "string ";
                                break;
                            case VarState.Types.Int:
                                firstLine += "int ";
                                break;
                            default:
                                firstLine += "dynamic ";
                                break;
                        }
                        firstLine += parameters[i];
                        firstLine += " = ";
                        firstLine += defaultParameters[parameters[i]];
                    }
                }
                // Case of a positional (usual) parameter.
                // This is also a place where we place the used types in a parent
                // constructor call.
                else
                {
                    switch (usedParameterTypes[i])
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
                        case VarState.Types.Overriden:
                            firstLine += overridenParameterTypes[parameters[i]];
                            break;
                        default:
                            firstLine += "dynamic ";
                            break;
                    }
                    firstLine += parameters[i];
                }
            }
            firstLine += ")";
            // This is a case when we invoke the base class constructor.
            if (baseConstructorInitializerList.Count != 0)
            {
                firstLine += " : base(";
                for (int i = 0; i < baseConstructorInitializerList.Count; ++i)
                {
                    if (i != 0)
                    {
                        firstLine += ", ";
                    }
                    firstLine += baseConstructorInitializerList[i];
                }
                firstLine += ")";
            }

            output.outputBuilder.commitIndentedLine(new IndentedLine(firstLine, 0));
            output.outputBuilder.commitIndentedLine(new IndentedLine("{", 1));
            
            // Declare the temporaries used for translation of the "else" block in for loops.
            for (int i = 0; i <= currentGeneratedElseBlockEntryNumber; ++i)
            {
                IndentedLine newLine = new IndentedLine("dynamic " +
                    "_generated_else_entry_" + i + " = true;", 0);
                output.outputBuilder.commitIndentedLine(newLine);
            }
            foreach (var indentedLine in statements.lines)
            {
                output.outputBuilder.commitIndentedLine(indentedLine);
            }
            // Commit each internal function (at the end).
            foreach (var internalFunc in internalFunctions)
            {
                internalFunc.ResolveGlobalVariables();

                // Internal functions are not public.
                internalFunc.isPublic = false;
                internalFunc.CommitToOutput();
            }

            output.outputBuilder.commitIndentedLine(new IndentedLine("", -1));
            output.outputBuilder.commitIndentedLine(new IndentedLine("}", 0));
        }
    }
    public void CommitGeneratedFunctionInScope()
    {
        foreach (var func in pendingGeneratedFunctionsInScope)
        {
            statements.lines.Add(new IndentedLine("dynamic " + func.name + "()", 0));
            statements.lines.Add(new IndentedLine("{", 1));
            foreach (var line in func.statements.lines)
            {
                statements.lines.Add(line);
            }
            statements.lines.Add(new IndentedLine("", -1));
            statements.lines.Add(new IndentedLine("}", 0));
        }
        pendingGeneratedFunctionsInScope.Clear();
    }
    public void CommitGeneratedFunctionInScope(BlockModel result)
    {
        foreach (var func in pendingGeneratedFunctionsInScope)
        {
            result.lines.Add(new IndentedLine("dynamic " + func.name + "()", 0));
            result.lines.Add(new IndentedLine("{", 1));
            foreach (var line in func.statements.lines)
            {
                result.lines.Add(line);
            }
            result.lines.Add(new IndentedLine("", -1));
            result.lines.Add(new IndentedLine("}", 0));
        }
        pendingGeneratedFunctionsInScope.Clear();
    }
}