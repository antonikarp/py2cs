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
1. Set current working directory to the root directory of the repository.
2. Clean directories:
    - make sure that the .cs files from the previous runs are deleted.
    - to clean directories from previous test runs, execute:
        $ cd tests
        $ ./clean_test_dirs.sh
        $ cd ..
3. Run the translator by the following command:
        $ dotnet run <input_path> <output_dir>
    where:
    - <input_path> is a path to the input script. If there are multiple scripts
        with import dependencies between each other, provide only the path to the main file
    - <output_dir> is a path to a directory where the resulting programs will be placed.
    example:
        $ dotnet run ./input/example.py ./output       
4. Possible errors in translation will be displayed on the console.
********************************************************************************
");
        }
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "help")
            {
                PrintHelp();
                return;
            }

            if (args.Length != 2)
            {
                return;
            }

            List<string> cleanedScriptNames = new List<string>();
            List<string> scriptNames = new List<string>();

            string currentDirectory = Directory.GetCurrentDirectory();

            string input_path = args[0];

            string[] tokensSplitBySlash = input_path.Split('/');
            string[] tokensSplitByDot = tokensSplitBySlash[tokensSplitBySlash.Length - 1].Split('.');
            string name = tokensSplitByDot[0];
            string output_path_cs = args[1] + "/" + name + ".cs";

            Translator translator = new Translator();

            translator.Translate(input_path, output_path_cs, new List<string>());
        }
    }
}
