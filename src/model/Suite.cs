using System.Text;
using System.Collections.Generic;

// This class is a model for a collection of statements for example in if statement
public class Suite
{
    public List<string> lines;
    public Suite()
    {
        lines = new List<string>();
    }
}