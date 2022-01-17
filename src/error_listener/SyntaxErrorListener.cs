using System;
using System.IO;
using Antlr4.Runtime;
public class SyntaxErrorListener : BaseErrorListener
{
    public bool isSyntaxError;
    public SyntaxErrorListener()
    {
        isSyntaxError = false;
    }
    public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        // Ignore if the offending symbol is <EOF>.
        if (offendingSymbol.Text != "<EOF>")
        {
            isSyntaxError = true;
        }
    }
}