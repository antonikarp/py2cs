using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
namespace py2cs
{
    public class Translator
    {
        public OutputVisitor outputVisitor;
        public static string input_path;
        public static string output_path;
        public static List<string> importedFilenames = new List<string>();

        public bool Translate(string input_path, string output_path, string moduleName)
        {
            Translator.input_path = input_path;
            Translator.output_path = output_path;
            string text = File.ReadAllText(input_path);
            ICharStream stream = CharStreams.fromString(text);
            ITokenSource lexer = new Python3Lexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            Python3Parser parser = new Python3Parser(tokens);
            parser.BuildParseTree = true;
            // Add a custome error listener for syntax errors.
            parser.RemoveErrorListeners();
            SyntaxErrorListener syntaxErrorListener = new SyntaxErrorListener();
            parser.AddErrorListener(syntaxErrorListener);

            // Start at the root, which is a node 'file_input'
            IParseTree tree = parser.file_input();

            if (syntaxErrorListener.isSyntaxError)
            {
                string textFilePath = output_path;
                textFilePath = textFilePath.Replace(".cs", ".txt");
                string content = "Syntax error: Unable to parse the input.";
                File.WriteAllText(textFilePath, content);
                return false;
            }
            outputVisitor = new OutputVisitor(moduleName);
            // Check if there are any not implemented features.
            NotImplementedCheckVisitor notImplementedCheckVisitor = new NotImplementedCheckVisitor();
            notImplementedCheckVisitor.Visit(tree);
            if (notImplementedCheckVisitor.isNotImplemented)
            {
                // We have a language feature not handled by the tool.
                // Write a .txt file with a message to the user. It is used
                // also by scripts which checks the results of tests.
                string textFilePath = output_path;
                textFilePath = textFilePath.Replace(".cs", ".txt");
                string content = "Not handled: With used language constructs the translation couldn't be performed.";
                File.WriteAllText(textFilePath, content);
                return false;
            }
            // Translate the program.
            outputVisitor.Visit(tree);
            File.WriteAllText(output_path, outputVisitor.state.output.ToString());
            return true;
        }
        public void Compile(string filename, string directory, string subDirectory)
        {
            ProcessStartInfo compiler = new ProcessStartInfo();
            // This is for Mac OS X.
            compiler.FileName = "/Library/Frameworks/Mono.framework/Versions/Current/Commands/csc";
            string arguments = filename;
            foreach (var importedFilename in Translator.importedFilenames)
            {
                arguments += " ";
                arguments += importedFilename;
            }
            compiler.Arguments = arguments;
            string workingDirectory = Directory.GetCurrentDirectory();
            workingDirectory += "/../../";
            string[] subDirectoryTokens = subDirectory.Split("/");
            // Append additional "../" if we are deeper in the subdirectory.
            if (subDirectory != "")
            {
                for (int i = 0; i < subDirectoryTokens.Length; ++i)
                {
                    workingDirectory += "../";
                }
            }
            workingDirectory += "generated/";
            workingDirectory += directory;
            compiler.WorkingDirectory = workingDirectory;
            var process = Process.Start(compiler);
            process.WaitForExit();
        }
    }
}