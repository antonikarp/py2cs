using System.Collections.Generic;
public class Library
{
    public LibConsoleExt libConsoleExt;
    public Output output;
    public Library(Output _output)
    {
        output = _output;
        libConsoleExt = new LibConsoleExt(output);
    }
    
}