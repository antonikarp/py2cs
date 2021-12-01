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
            string text = File.ReadAllText("../../../input/example.py");
            ICharStream stream = CharStreams.fromString(text);
            ITokenSource lexer = new Python3Lexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            Python3Parser parser = new Python3Parser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.file_input();
            OutputVisitor outputVisitor = new OutputVisitor();
            outputVisitor.Visit(tree);
            File.WriteAllText("../../../output/example.cs", outputVisitor.output.ToString());
        }
    }
}
