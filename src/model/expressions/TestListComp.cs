using System.Text;
using System.Collections.Generic;

// This class is a model for a list comprehesion. It used for declaring lists.
// For example: a = [2, 3, 4]
public class TestListComp
{
    public List<string> tokens;
    public TestListComp()
    {
        tokens = new List<string>();
    }
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < tokens.Count; ++i)
        {
            sb.Append(tokens[i]);
        }
        return sb.ToString();
    }

}