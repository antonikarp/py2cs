using System.Collections.Generic;

// This is a model for a list of expression. The order of elements matters.
// Example:
//
// for x, y in tuple_collection
//
// expressions[0] == "x"
// expressions[1] == "y"
public class ExprlistModel
{
    public List<string> expressions;
    public ExprlistModel()
    {
        expressions = new List<string>();
    }
}