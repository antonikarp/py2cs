using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.IO;
namespace py2cs
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = File.ReadAllText(args[0]);
            ICharStream stream = CharStreams.fromString(text);
            ITokenSource lexer = new LabeledExprLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            LabeledExprParser parser = new LabeledExprParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.prog();
            ParseTreeWalker walker = new ParseTreeWalker();
            EvalVisitor eval = new EvalVisitor();
            eval.Visit(tree);
        }
    }
}
