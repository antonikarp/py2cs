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

        public bool Translate(string input_path, string output_path, List<string> moduleNames)
        {
            Console.WriteLine(input_path + ":");
            Translator.input_path = input_path;
            Translator.output_path = output_path;

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
                string textFilePath = output_path;
                textFilePath = textFilePath.Replace(".cs", ".txt");
                File.WriteAllText(textFilePath, content);
                Console.WriteLine(content);

                return false;
            }
            outputVisitor = new OutputVisitor(moduleNames);
            // Check if there are any not implemented features.
            NotImplementedCheckVisitor notImplementedCheckVisitor = new NotImplementedCheckVisitor();
            try
            {
                notImplementedCheckVisitor.Visit(tree);
            }
            catch (NotImplementedException ex)
            {
                // We have a language feature not handled by the tool.
                // Write a .txt file with a message to the user. It is used
                // also by scripts which checks the results of tests.
                string content = "Not handled: " + ex.message;
                string textFilePath = output_path;
                textFilePath = textFilePath.Replace(".cs", ".txt");
                File.WriteAllText(textFilePath, content);
                Console.WriteLine(content);
                return false;
            }

            try
            {
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
            }
            catch (IncorrectInputException ex)
            {
                string content = "Error: incorrect input. " + ex.message;
                string textFilePath = output_path;
                textFilePath = textFilePath.Replace(".cs", ".txt");
                File.WriteAllText(textFilePath, content);
                Console.WriteLine(content);
            }
            catch (Exception)
            {
                Console.WriteLine("Error in translating: " + output_path);
            }
            return false;
        }
    }
}