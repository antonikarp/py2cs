using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.IO;
using System.Threading.Tasks;
namespace py2cs
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = File.ReadAllText(args[0]);
            ICharStream stream = CharStreams.fromString(text);
            ITokenSource lexer = new Python3Lexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            Python3Parser parser = new Python3Parser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.file_input();
            ParseTreeWalker walker = new ParseTreeWalker();
            Python3Translator translator = new Python3Translator();
            walker.Walk(translator, tree);
            File.WriteAllText("out.cs", translator.output.ToString());
        }
    }
}
