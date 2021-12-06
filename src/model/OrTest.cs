using System.Text;
using System.Collections.Generic;

// This class is a model for a expression composed with the logical operator "or".
public class OrTest
{
    public List<string> tokens;
    public OrTest()
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