using System.Text;
using System.Collections.Generic;
// This class is a model for a class.
public class Class
{
    public string name;
    public BlockModel fieldDecl;
    public List<string> fields;
    public Function mainMethod;
    public Stack<Function> currentFunctions;
    public List<Function> functions;
    public List<Class> internalClasses;
    public Output output;
    public Class parentClass;
    // constructorSignatures maps the number of parameters in a constructor
    // to the constructor. It is enough, because in Python variables are dynamically
    // typed.
    public Dictionary<int, Function> constructorSignatures;

    public string libraryParentClassName;

    // This is for variables defined in the main scope.
    public BlockModel staticFieldDeclarations;
    // This set is used when there is a declaration of a variable in a function
    // with the same identifier as some static field member.
    public HashSet<string> staticFieldIdentifiers;

    // This indicates how many functions returning bool which represent a chained
    // expression have been generated.
    public int currentGeneratedChainedComparisonNumber = -1;

    public bool isStatic;
    public bool isPartial;
    public Class(Output _output)
    {
        output = _output;
        mainMethod = new Function(output);
        currentFunctions = new Stack<Function>();
        functions = new List<Function>();
        fieldDecl = new BlockModel();
        fields = new List<string>();
        internalClasses = new List<Class>();
        parentClass = null;
        constructorSignatures = new Dictionary<int, Function>();
        libraryParentClassName = "";
        staticFieldDeclarations = new BlockModel();
        staticFieldIdentifiers = new HashSet<string>();
        isStatic = false;
    }
    public void CommitToOutput()
    {
        string firstLine = "public ";
        if (isStatic)
        {
            firstLine += "static ";
        }
        if (isPartial)
        {
            firstLine += "partial ";
        }
        firstLine += "class ";
        firstLine += name;
        if (parentClass != null)
        {
            firstLine += " : ";
            firstLine += parentClass.name;
        }
        // To be used only in library classes.
        else if (libraryParentClassName != "")
        {
            firstLine += " : ";
            firstLine += libraryParentClassName;
        }
        output.outputBuilder.commitIndentedLine(new IndentedLine(firstLine, 0));
        output.outputBuilder.commitIndentedLine(new IndentedLine("{", 1));
        // Field declarations (the class is other than Program)
        foreach (var line in fieldDecl.lines)
        {
            output.outputBuilder.commitIndentedLine(line);
        }

        // Static field declaration (the class is Program)
        foreach (var line in staticFieldDeclarations.lines)
        {
            output.outputBuilder.commitIndentedLine(line);
        }

        // Print all internal classes.
        foreach (var internalCls in internalClasses)
        {
            internalCls.CommitToOutput();
        }

        foreach (var func in functions)
        {
            func.CommitToOutput();
        }

        output.outputBuilder.commitIndentedLine(new IndentedLine("", -1));
        output.outputBuilder.commitIndentedLine(new IndentedLine("}", 0));

    }
    public void GenerateConstructor(List<VarState.Types> argTypes)
    {
        // We invoke this function only in a constructor of a derived class.
        if (parentClass != null)
        {
            int n = argTypes.Count;
            Function constructor = constructorSignatures[n];

            // Avoid duplicates.
            bool isDuplicate = false;
            foreach (var paramTypes in constructor.usedParameterTypesInConstructor)
            {
                if (paramTypes.Count == n)
                {
                    isDuplicate = true;
                    for (int i = 0; i < n; ++i)
                    {
                        if (paramTypes[i] != argTypes[i])
                        {
                            isDuplicate = false;
                        }
                    }
                    if (isDuplicate)
                    {
                        break;
                    }
                }
            }
            if (!isDuplicate)
            {
                constructor.usedParameterTypesInConstructor.Add(argTypes);
            }
        }
    }

}