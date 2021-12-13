using Antlr4.Runtime.Misc;

// This is a visitor used for translating function "range()" in Python.
// There can be 1, 2 or 3 arguments supplied.
public class RangeTrailerVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public RangeTrailerVisitor(State _state)
    {
        state = _state;
    }
    public override LineModel VisitTrailer([NotNull] Python3Parser.TrailerContext context)
    {
        result = new LineModel();
        // We have a form: range(a)
        // 0 - start value, a - stop value, 1 - step value
        // This is translated to Enumerable.Range(0, 3);
        if (context.arglist().ChildCount == 1)
        {
            result.tokens.Add("(");
            result.tokens.Add("0");
            result.tokens.Add(",");
            OrTestVisitor newVisitor = new OrTestVisitor(state);
            context.arglist().GetChild(0).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
            result.tokens.Add(")");
        }
        // We have a form: range(a, b)
        // a - start value, b - stop value, 1 - step value
        // This is translated to Enumerable.Range(a, (b)-(a));
        if (context.arglist().ChildCount == 3)
        {
            result.tokens.Add("(");
            OrTestVisitor firstArgVisitor = new OrTestVisitor(state);
            context.arglist().GetChild(0).Accept(firstArgVisitor);
            OrTestVisitor secondArgVisitor = new OrTestVisitor(state);
            context.arglist().GetChild(2).Accept(secondArgVisitor);
            for (int i = 0; i < firstArgVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(firstArgVisitor.result.tokens[i]);
            }
            result.tokens.Add(",");
            result.tokens.Add("(");
            for (int j = 0; j < secondArgVisitor.result.tokens.Count; ++j)
            {
                result.tokens.Add(secondArgVisitor.result.tokens[j]);
            }
            result.tokens.Add(")");
            result.tokens.Add("-");
            result.tokens.Add("(");
            for (int i = 0; i < firstArgVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(firstArgVisitor.result.tokens[i]);
            }
            result.tokens.Add(")");
            result.tokens.Add(")");
        }
        // We have a form: range(a, b, c)
        // a - start value, b - stop value, c - step value
        // This is translated to Enumerable.Range(a, ((b)-(a))/(c)).Select(x => x * (c))
        // We need to add System.Linq to the using directives in State
        if (context.arglist().ChildCount == 5)
        {
            result.tokens.Add("(");
            OrTestVisitor firstArgVisitor = new OrTestVisitor(state);
            context.arglist().GetChild(0).Accept(firstArgVisitor);
            OrTestVisitor secondArgVisitor = new OrTestVisitor(state);
            context.arglist().GetChild(2).Accept(secondArgVisitor);
            OrTestVisitor thirdArgVisitor = new OrTestVisitor(state);
            context.arglist().GetChild(4).Accept(thirdArgVisitor);
            state.classState.usingDirs.Add("System.Linq");
            for (int i = 0; i < firstArgVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(firstArgVisitor.result.tokens[i]);
            }
            result.tokens.Add(",");
            result.tokens.Add("(");
            result.tokens.Add("(");
            for (int j = 0; j < secondArgVisitor.result.tokens.Count; ++j)
            {
                result.tokens.Add(secondArgVisitor.result.tokens[j]);
            }
            result.tokens.Add(")");
            result.tokens.Add("-");
            result.tokens.Add("(");
            for (int i = 0; i < firstArgVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(firstArgVisitor.result.tokens[i]);
            }
            result.tokens.Add(")");
            result.tokens.Add(")");
            result.tokens.Add("/");
            result.tokens.Add("(");
            for (int k = 0; k < thirdArgVisitor.result.tokens.Count; ++k)
            {
                result.tokens.Add(thirdArgVisitor.result.tokens[k]);
            }
            result.tokens.Add(")");
            result.tokens.Add(")");
            result.tokens.Add(".");
            result.tokens.Add("Select(x => x*");
            result.tokens.Add("(");
            for (int k = 0; k < thirdArgVisitor.result.tokens.Count; ++k)
            {
                result.tokens.Add(thirdArgVisitor.result.tokens[k]);
            }
            result.tokens.Add(")");
            result.tokens.Add(")");

        }
        
        return base.VisitTrailer(context);
    }




}