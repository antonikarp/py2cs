using System;
public class NotImplementedException : Exception
{
    public string message;
    public NotImplementedException(string _message)
    {
        message = _message;
    }
}