public class LibIsOperator
{
    public string text;
    public Library library;
    public LibIsOperator(Library _library)
    {
        library = _library;
        text = @"
public static class IsOperator
{
    public static bool Compare(dynamic x, dynamic y)
    {
        if (x.GetType().IsValueType && y.GetType().IsValueType && x == y)
        {
            return true;
        }
        if (Object.ReferenceEquals(x, y))
        {
            return true;
        }
        return false;
    }
};";
        library.availableClasses["IsOperator"] = text;
    }
}