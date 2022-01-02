using System.Text;
using System.Collections.Generic;

// This class is a model for a collection of statements for example in if statement.
public class Suite
{
    public List<IndentedLine> lines;
    public Suite()
    {
        lines = new List<IndentedLine>();
    }
}