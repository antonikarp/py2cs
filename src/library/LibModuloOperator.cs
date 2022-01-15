public class LibModuloOperator
{
    public string text;
    public Library library;
    public LibModuloOperator(Library _library)
    {
        library = _library;
        text = @"
public static class ModuloOperator
{
    public static double Compute(dynamic x, dynamic y)
    {
        return x - Math.Floor((double)x / y) * y;
    }
}";
        library.availableClasses["ModuloOperator"] = text;
    }
}