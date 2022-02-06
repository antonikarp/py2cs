using Antlr4.Runtime.Misc;

public class TrailerConstructorCheckVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public bool isConstructor;
    public TrailerConstructorCheckVisitor(State _state)
    {
        isConstructor = true;
        state = _state;
    }
    public override LineModel VisitTrailer([NotNull] Python3Parser.TrailerContext context)
    {
        result = new LineModel();
        if (context.ChildCount == 2 && context.NAME() != null)
        {
            if (!state.output.allClassesNames.Contains(context.NAME().ToString()))
            {
                isConstructor = false;
            }
        }
        return result;
    }

}