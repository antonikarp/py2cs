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
        public static List<string> importedFilenames;
        public void Translate(string input_path, string output_path, string moduleName)
        {
            Translator.importedFilenames = new List<string>();
            Translator.input_path = input_path;
            Translator.output_path = output_path;
            string text = File.ReadAllText(input_path);
            ICharStream stream = CharStreams.fromString(text);
            ITokenSource lexer = new Python3Lexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            Python3Parser parser = new Python3Parser(tokens);
            parser.BuildParseTree = true;
            // Start at the root, which is a node 'file_input'
            IParseTree tree = parser.file_input();
            outputVisitor = new OutputVisitor(moduleName);
            // Translate the program.
            outputVisitor.Visit(tree);
            File.WriteAllText(output_path, outputVisitor.state.output.ToString());
        }
        public void Compile(string filename)
        {
            ProcessStartInfo compiler = new ProcessStartInfo();
            // This is for Mac OS X.
            compiler.FileName = "/Library/Frameworks/Mono.framework/Versions/Current/Commands/csc";
            string arguments = filename;
            foreach (var importedFilename in Translator.importedFilenames)
            {
                arguments += " ";
                arguments += importedFilename ;
            }
            compiler.Arguments = arguments;
            compiler.WorkingDirectory = Directory.GetCurrentDirectory() + "/../generated";
            var process = Process.Start(compiler);
            process.WaitForExit();
        }
    }
}