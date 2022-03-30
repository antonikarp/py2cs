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
2. On Mac OS/Linux to clean directories from previous test runs, execute:
        $ cd tests
        $ ./clean_test_dirs.sh
        $ cd ..
3. Run the translator by the following command (on all platforms):
        $ dotnet run <input_path> <output_dir>
   where:
   - <input_path> is a path to the input script. If there are multiple scripts
    with import dependencies between each other, provide only the path to the main file.
   - <output_dir> is a path to the directory where the resulting programs will be placed.

   Example:
        $ dotnet run ./input/example.py ./output
   The '.py' in the <input_path> can be skipped:
        $ dotnet run ./input/example ./output

   By default, the contents of the <output_dir> will be deleted before the translation. However,
   if you specify a different <output_dir> the generated files will remain in the old
   <output_dir>. Before the translation you still need to manually delete such files.

   If you wish to disable this deletion, invoke the translator with flag --nodelete:
        $ dotnet run ./input/example.py ./output --nodelete
4. On Windows forward slashes ('/') can be replaced by backward slashes ('\').
   After build, the translator can be invoked from the 'bin' directory by:
        > .\py2cs input\example.py output
   All such paths are relative to the current working directory. 
5. Possible errors in translation will be displayed on the console.
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
            else if (args.Length <= 1)
            {
                Console.WriteLine("Too few arguments. Translation aborted.");
                return;
            }

            bool isNoDeleteSet = false;
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i] == "--nodelete")
                {
                    isNoDeleteSet = true;
                }
            }

            List<string> cleanedScriptNames = new List<string>();
            List<string> scriptNames = new List<string>();

            string currentDirectory = Directory.GetCurrentDirectory();

            input_path = args[0];

            // Substitute backslahes by forward slashes to make the program run on Windows
            input_path = input_path.Replace('\\', '/');
            args[1] = args[1].Replace('\\', '/');

            // Add .py to the input path if there is no such extension
            if (!input_path.EndsWith(".py"))
            {
                input_path += ".py";
            }

            // Check if the file given as a input path exists.
            if (!File.Exists(input_path))
            {
                Console.WriteLine("The file: " + input_path + " doesn't exist. Translation aborted.");
                return;
            }

            // Check if the output folder exists. If not, create it.
            if (!Directory.Exists(args[1]))
            {
                try
                {
                    Directory.CreateDirectory(args[1]);
                }
                // The path is not valid. One of characters \/:*?"<>| is present.
                catch (System.IO.IOException)
                {
                    Console.WriteLine("The path for the output folder: " + args[1]
                        + "is not valid. Translation aborted.");
                    return;
                }
            }

            // Empty contents of the output folder with a possible exception of .gitignore file
            if (!isNoDeleteSet)
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(args[1]);
                foreach (FileInfo file in di.GetFiles())
                {
                    if (file.Name != ".gitignore")
                    {
                        file.Delete();
                    }
                }
            }

            string[] tokensSplitBySlash = input_path.Split('/');
            string[] tokensSplitByDot = tokensSplitBySlash[tokensSplitBySlash.Length - 1].Split('.');
            string name = tokensSplitByDot[0];
            string output_path_cs = args[1] + "/" + name + ".cs";

            Translator translator = new Translator();

            translator.Translate(input_path, output_path_cs, new List<string>());
        }
    }
}
