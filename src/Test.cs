﻿using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.IO;
using System.Threading.Tasks;
namespace py2cs
{
    public class Translator
    {
        public void Translate(string input_path, string output_path)
        {
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