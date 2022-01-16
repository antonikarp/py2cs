public class LibDivideByZero
{
    public string text;
    public Library library;
    public LibDivideByZero(Library _library)
    {
        library = _library;
        text = @"
public static class DivideByZero
{
    public static int Evaluate()
    {
        throw new DivideByZeroException();
    }
};";
        library.availableClasses["DivideByZero"] = text;
    }

}