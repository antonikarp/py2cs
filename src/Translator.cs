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

        public void Translate(string input_path, string output_path, List<string> moduleNames)
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
                string content = "Syntax error: (Line " + syntaxErrorListener.line +
                    ", Pos: " + syntaxErrorListener.pos + ") Unable to parse the input.";
                string textFilePath = output_path;
                textFilePath = textFilePath.Replace(".cs", ".txt");
                File.WriteAllText(textFilePath, content);
                Console.WriteLine(content);
                return;
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
                string content = ex.ToString();
                string textFilePath = output_path;
                textFilePath = textFilePath.Replace(".cs", ".txt");
                File.WriteAllText(textFilePath, content);
                Console.WriteLine(content);
                return;
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
                return;
            }
            catch (IncorrectInputException ex)
            {
                string content = ex.ToString();
                string textFilePath = output_path;
                textFilePath = textFilePath.Replace(".cs", ".txt");
                File.WriteAllText(textFilePath, content);
                Console.WriteLine(content);
            }
            catch (NotImplementedException ex)
            {
                // We have a language feature not handled by the tool.
                // Write a .txt file with a message to the user. It is used
                // also by scripts which checks the results of tests.
                string content = ex.ToString();
                string textFilePath = output_path;
                textFilePath = textFilePath.Replace(".cs", ".txt");
                File.WriteAllText(textFilePath, content);
                Console.WriteLine(content);
                return;
            }
            catch (Exception)
            {
                Console.WriteLine("Error in translating: " + output_path);
            }
        }
    }
}