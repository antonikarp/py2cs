using System;
using System.IO;
using Antlr4.Runtime;
public class SyntaxErrorListener : BaseErrorListener
{
    public bool isSyntaxError;
    public int line;
    public int pos;
    public SyntaxErrorListener()
    {
        isSyntaxError = false;
        line = -1;
        pos = -1;
    }
    public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        // Ignore if the offending symbol is <EOF>.
        if (offendingSymbol.Text != "<EOF>")
        {
            isSyntaxError = true;
            this.line = line;
            pos = charPositionInLine;
        }
    }
}