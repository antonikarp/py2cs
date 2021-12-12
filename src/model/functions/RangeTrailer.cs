using System.Text;
using System.Collections.Generic;
public class RangeTrailer
{
    public List<string> tokens;
    public RangeTrailer()
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
