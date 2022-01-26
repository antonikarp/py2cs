using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace py2cs
{
    public class Program
    {
        public static string input_path;
        public static string output_path;
        static void PrintHelp()
        {
            Console.WriteLine(@"
********************************************************************************
py2cs - a source-to-source translator from Python to C# using ANTLR parser generator.

To perform translation:
1. Clean directories:
    - make sure that the ./output directory is empty
    - to clean directories from previous test runs, execute tests/clean_test_dirs.sh
2. Copy all Python scripts that will be translated to ./input directory. If there are
import dependencies, append ""_0"" to the name of the main file
3. Run the translator by the following command:
        $ dotnet run
   If you wish to additionaly compile the obtained .cs sources run:
        $ dotnet run compile
4. Possible errors in translation will be displayed on the console.
********************************************************************************
");
        }
        static void Main(string[] args)
        {
            bool shouldBeCompiled = false;
            if (args.Length == 1 && args[0] == "help")
            {
                PrintHelp();
                return;
            }
            else if (args.Length == 1 && args[0] == "compile")
            {
                shouldBeCompiled = true;
            }

            List<string> cleanedScriptNames = new List<string>();
            List<string> scriptNames = new List<string>();

            string input_directory = Directory.GetCurrentDirectory() +  "/input/";
            string output_directory = Directory.GetCurrentDirectory() + "/output/";

            string[] paths = Directory.GetFiles(input_directory);

            foreach (string path in paths)
            {
                string[] tokens = path.Split("/");
                string potentialFilename = tokens[tokens.Length - 1];
                if (potentialFilename.EndsWith(".py"))
                {
                    string[] beforeDot = potentialFilename.Split(".");
                    scriptNames.Add(beforeDot[0]);
                }
            }

            foreach (string scriptName in scriptNames)
            {
               
                // If there are import dependencies between files,
                // name of one of the files must end with "_0"
                
                if (scriptNames.Count > 1 && scriptName.Length >= 2 &&
                    (scriptName[scriptName.Length - 2] != '_' ||
                    scriptName[scriptName.Length - 1] != '0'))
                {
                    continue;
                }
                cleanedScriptNames.Add(scriptName);
                    
            }
            foreach (string name in cleanedScriptNames)
            {
                string input_path = input_directory + name + ".py";
                string output_path_cs = output_directory + name + ".cs";
                string output_path_exe = output_directory + name + ".exe";
                Translator translator = new Translator(false);
                if (translator.Translate(input_path, output_path_cs, new List<string>()) && shouldBeCompiled)
                {
                    translator.Compile(output_directory, name + ".cs");
                }
            }
        }
    }
}
