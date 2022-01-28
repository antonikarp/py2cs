using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
namespace py2cs
{
    public class Translator
    {
        public OutputVisitor outputVisitor;
        public static string input_path;
        public static string output_path;
        public static List<string> importedFilenames = new List<string>();
        public bool writeMessagesToFile;
        public string mode;

        public Translator(bool _writeMessagesToFile)
        {
            writeMessagesToFile = _writeMessagesToFile;
            mode = "";
        }
        public int CheckForErrorsInScript(string input_path)
        {
            // Returns 0 if the execution has succeeded.
            // Returns -1 if the script writes anything to stderr.
            // Returns -2 if the script runs out of memory during execution

            // In case of 'input', the program would hang, because there is no
            // input provided. In case of 'differences' even when the script itself
            // is not correct, the translation can produce a correct program.
            if (mode == "input" || mode == "differences")
            {
                return 0;
            }
            ProcessStartInfo pythonShell = new ProcessStartInfo();
            // This is the location of the python3 executable.
            // (1) -----> Important! Please update it so that the path is correct on your machine.
            pythonShell.FileName = "/Users/antoni.karpinski/opt/anaconda3/bin/python3";
            // Remove assert statements.
            string arguments = "-O ";
            arguments += input_path;
            pythonShell.Arguments = arguments;
            string workingDirectory = Directory.GetCurrentDirectory();
            pythonShell.WorkingDirectory = workingDirectory;
            // Check if there is anything on the stderr. If yes, then the script
            // is incorrect.
            pythonShell.RedirectStandardError = true;
            // Ignore stdout.
            pythonShell.RedirectStandardOutput = true;
            var process = Process.Start(pythonShell);
            bool outOfMemory = false;
            while (!process.HasExited && !outOfMemory)
            {
                Thread.Sleep(100);
                if (process.HasExited)
                {
                    continue;
                }
                else if (process.VirtualMemorySize64 > 50000000)
                {
                    outOfMemory = true;
                    continue;
                }
            }
            if (outOfMemory)
            {
                return -2;
            }
            StreamReader stderrReader = process.StandardError;
            string stderr = stderrReader.ReadToEnd();
            if (stderr != "")
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public bool Translate(string input_path, string output_path, List<string> moduleNames, string _mode)
        {
            Translator.input_path = input_path;
            Translator.output_path = output_path;
            mode = _mode;
            // First, check if the script is correct
            int resultCheckForErrors = CheckForErrorsInScript(input_path);
            if (resultCheckForErrors < 0)
            {
                string content;
                if (resultCheckForErrors == -1)
                {
                    content = "Error: the script is incorrect, unable to translate.";
                }
                else
                {
                    content = "Error: Out of memory.";
                }

                if (writeMessagesToFile)
                {
                    string textFilePath = output_path;
                    textFilePath = textFilePath.Replace(".cs", ".txt");
                    File.WriteAllText(textFilePath, content);
                }
                else
                {
                    Console.WriteLine(content);
                }
                return false;
            }

            string text = File.ReadAllText(input_path);
            ICharStream stream = CharStreams.fromString(text);
            ITokenSource lexer = new Python3Lexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            Python3Parser parser = new Python3Parser(tokens);
            parser.BuildParseTree = true;
            // Add a custom error listener for syntax errors.
            parser.RemoveErrorListeners();
            SyntaxErrorListener syntaxErrorListener = new SyntaxErrorListener();
            parser.AddErrorListener(syntaxErrorListener);

            // Start at the root, which is a node 'file_input'
            IParseTree tree = parser.file_input();

            if (syntaxErrorListener.isSyntaxError)
            {
                string content = "Syntax error: Unable to parse the input.";
                if (writeMessagesToFile)
                {
                    string textFilePath = output_path;
                    textFilePath = textFilePath.Replace(".cs", ".txt");
                    File.WriteAllText(textFilePath, content);
                }
                else
                {
                    Console.WriteLine(content);
                }
                return false;
            }
            outputVisitor = new OutputVisitor(moduleNames);
            // Check if there are any not implemented features.
            NotImplementedCheckVisitor notImplementedCheckVisitor = new NotImplementedCheckVisitor();
            notImplementedCheckVisitor.Visit(tree);
            if (notImplementedCheckVisitor.isNotImplemented)
            {
                // We have a language feature not handled by the tool.
                // Write a .txt file with a message to the user. It is used
                // also by scripts which checks the results of tests.
                string content = "Not handled: With used language constructs the translation couldn't be performed.";
                if (writeMessagesToFile)
                {
                    string textFilePath = output_path;
                    textFilePath = textFilePath.Replace(".cs", ".txt");
                    File.WriteAllText(textFilePath, content);
                }
                else
                {
                    Console.WriteLine(content);
                }
                return false;
            }

            //try
            //{
                // Translate the program.
                outputVisitor.Visit(tree);
                File.WriteAllText(output_path, outputVisitor.state.output.ToStringMain());
                // Write used library classes to a separate file.
                // Only when we are in the main file, so that it is not duplicated
                // when performing imports.
                if (outputVisitor.state.output.moduleNames.Count == 0)
                {
                    string outputPathLib = output_path.Replace(".cs", "_lib.cs");
                    File.WriteAllText(outputPathLib, outputVisitor.state.output.ToStringLib());
                }
                Console.WriteLine("Translation successful.");
                return true;
            //}
            //catch (Exception)
            //{
            //    Console.WriteLine("Error in translating: " + output_path);
            //}
            //return false;
        }
        public void Compile(string outputDirectory, string filename)
        {
            ProcessStartInfo compiler = new ProcessStartInfo();
            // This is for Mac OS X.
            // (2) -----> Important! Please update it so that the path is correct on your machine.
            compiler.FileName = "/Library/Frameworks/Mono.framework/Versions/Current/Commands/csc";
            string arguments = filename;
            // Add the attached library file.
            string libFilename = filename.Replace(".cs", "_lib.cs");
            arguments += " ";
            arguments += libFilename;
            foreach (var importedFilename in Translator.importedFilenames)
            {
                arguments += " ";
                arguments += importedFilename;
            }
            
            compiler.Arguments = arguments;
            compiler.WorkingDirectory = outputDirectory;
            var process = Process.Start(compiler);
            process.WaitForExit();

            // Clear the static importedFileNames list
            importedFilenames.Clear();
        }
    }
}