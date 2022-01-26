using System.Text;
using System.Collections.Generic;

// This class is a model for the entire program.
// For now, it translates expressions to the Main method in the Program class.
public class Output
{
    public List<IndentedLine> internalLines;
    public int indentationLevel = 0;
    public OutputBuilder outputBuilder;
    public OutputBuilder outputBuilderLib;
    public Stack<Class> currentClasses;
    public List<Class> classes;
    // allClassesNames is for checking if we have a constructor call (we need to
    // take into account also the nested classes).
    public List<string> allClassesNames;
    public Dictionary<string, Class> namesToClasses;
    // usingDirs - outside the library file
    // usingDirsLib - for the library file
    public HashSet<string> usingDirs;
    public HashSet<string> usingDirsLib;
    // This indicates whether the file is translated during processing an "import"
    // statement.
    public List<string> moduleNames;
    // This class holds objects of type Class that will be added to the main file
    // For example: ConsoleExt.
    public Library library;

    // This is a mapping from the name of the imported to used names throughout the program.
    public Dictionary<string, List<string>> usedNamesFromImport;

    public Output()
    {
        internalLines = new List<IndentedLine>();
        outputBuilder = new OutputBuilder();
        outputBuilderLib = new OutputBuilder();
        currentClasses = new Stack<Class>();
        usingDirs = new HashSet<string>();
        usingDirsLib = new HashSet<string>();

        classes = new List<Class>();
        allClassesNames = new List<string>();
        namesToClasses = new Dictionary<string, Class>();
        // Class Program.
        Class programClass = new Class(this);
        programClass.name = "Program";
        
        // Function Main - entry point
        Function mainFunction = new Function(this);
        mainFunction.isVoid = true;
        mainFunction.isStatic = true;
        mainFunction.name = "Main";
        mainFunction.parentClass = programClass;
        programClass.currentFunctions.Push(mainFunction);
        programClass.functions.Add(mainFunction);

        currentClasses.Push(programClass);
        classes.Add(programClass);
        allClassesNames.Add(programClass.name);
        namesToClasses[programClass.name] = programClass;

        // Add System in using directives.
        usingDirs.Add("System");

        usedNamesFromImport = new Dictionary<string, List<string>>();
        library = new Library(this);
        moduleNames = new List<string>();
    }

    public string ToStringMain()
    {
        // By default we include ConsoleExt.
        library.CommitConsoleExt();

        foreach (var dir in usingDirs)
        {
            outputBuilder.commitIndentedLine(new IndentedLine("using " + dir + ";", 0));
        }
        if (moduleNames.Count > 0)
        {
            Class lastModuleClass = new Class(this);
            Class outerClassToAdd = lastModuleClass;
            lastModuleClass.name = moduleNames[moduleNames.Count - 1];
            lastModuleClass.isStatic = true;
            lastModuleClass.isPartial = true;
            // Put each class into the module class.
            foreach (var cls in classes)
            {
                // Add each function from "Program" class to the moduleClass except
                // "Main" method.
                // But do not add the whole "Program" class itself.
                if (cls.name == "Program")
                {
                    foreach (var func in cls.functions)
                    {
                        if (func.name != "Main")
                        {
                            lastModuleClass.functions.Add(func);
                        }
                    }
                    continue;
                }
                lastModuleClass.internalClasses.Add(cls);
            }

            // Wrap the last module class in the classes that come before.
            Class nextClass = lastModuleClass;
            for (int i = moduleNames.Count - 2; i >= 0; --i)
            {
                Class curClass = new Class(this);
                curClass.name = moduleNames[i];
                curClass.isStatic = true;
                curClass.isPartial = true;
                curClass.internalClasses.Add(nextClass);
                outerClassToAdd = curClass;
            }
            // Add only the outermost class.
            classes.Clear();
            classes.Add(outerClassToAdd);
            allClassesNames.Add(outerClassToAdd.name);
            namesToClasses[outerClassToAdd.name] = outerClassToAdd;
        }

        foreach (var cls in classes)
        {
            cls.CommitToOutput();
        }
        return outputBuilder.output.ToString();
    }

    public string ToStringLib()
    {
        // Here we write the library functions to the main file.
        if (moduleNames.Count == 0)
        {
            foreach (var dir in usingDirsLib)
            {
                outputBuilderLib.commitIndentedLine(new IndentedLine("using " + dir + ";", 0));
            }
            foreach (var text in library.toCommit)
            {
                outputBuilderLib.commitRawCodeBlock(text);
            }
        }
        return outputBuilderLib.output.ToString();
    }
}
