using System;
public class IncorrectInputException : Exception
{
    public string message;
    public IncorrectInputException(string _message)
    {
        message = _message;
    }
}