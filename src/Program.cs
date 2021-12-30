﻿using Antlr4.Runtime;
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
            string input_path = "../../../input/example.py";
            string output_path = "../../../output/example.cs";
            if (args.Length == 2)
            {
                input_path = args[0];
                output_path = args[1];
            }
            string text = File.ReadAllText(input_path);
            ICharStream stream = CharStreams.fromString(text);
            ITokenSource lexer = new Python3Lexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            Python3Parser parser = new Python3Parser(tokens);
            parser.BuildParseTree = true;
            // Start at the root, which is a node 'file_input'
            IParseTree tree = parser.file_input();
            OutputVisitor outputVisitor = new OutputVisitor();
            // Translate the program.
            outputVisitor.Visit(tree);
            File.WriteAllText(output_path, outputVisitor.output.ToString());
        }
    }
}
