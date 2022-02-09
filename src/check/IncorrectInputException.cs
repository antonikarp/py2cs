using System;
public class IncorrectInputException : Exception
{
    public string message;
    public int line;
    public IncorrectInputException(string _message, int _line)
    {
        message = _message;
        line = _line;
    }
    public override string ToString()
    {
        return "Error: incorrect input. (Line " + line + ") " + message;
    }
}