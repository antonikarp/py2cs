using System;
public class NotImplementedException : Exception
{
    public string message;
    public int line;
    public NotImplementedException(string _message, int _line)
    {
        message = _message;
        line = _line;
    }
    public override string ToString()
    {
        return "Not handled: (Line " + line + ") " + message;
    }
}