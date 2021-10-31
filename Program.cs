using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;

namespace py2cs
{
    class Program
    {
        static void Main(string[] args)
        {
            ICharStream stream = CharStreams.fromString(args[0]);
            ITokenSource lexer = new ArrayInitLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            ArrayInitParser parser = new ArrayInitParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.init();
            ParseTreeWalker walker = new ParseTreeWalker();
            walker.Walk(new ShortToUnicodeString(), tree);
            Console.WriteLine();
        }
    }
}
