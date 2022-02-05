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
    - make sure that the ./output directory is empty
    - to clean directories from previous test runs, execute:
        $ cd tests
        $ ./clean_test_dirs.sh
        $ cd ..
3. Copy all Python scripts that will be translated to ./input directory. If there are
import dependencies, append ""_0"" to the name of the main file.
4. Run the translator by the following command:
        $ dotnet run
   If you wish to additionaly compile the obtained .cs sources run:
        $ dotnet run compile
5. Possible errors in translation will be displayed on the console.
********************************************************************************
");
        }
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                return;
            }
            if (args.Length == 1 && args[0] == "help")
            {
                PrintHelp();
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

            Translator translator = new Translator(false);

            translator.Translate(input_path, output_path_cs, new List<string>(), "");
           
        }
    }
}
