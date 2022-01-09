public class LibConsoleExt
{
    public string text;
    public Library library;
    public LibConsoleExt(Library _library)
    {
        library = _library;
        text = @"
public static class ConsoleExt
{
    public static string ToString(object obj)
    {
        return obj.ToString();
    }
    public static string ToString(List<dynamic> list)
    {
        StringBuilder result = new StringBuilder();
        result.Append(""["");
        for (int i = 0; i < list.Count; ++i)
        {
            if (i != 0) { result.Append("", ""); }
            result.Append(ToString(list[i]));
        }
        result.Append(""]"");
        return result.ToString();
    }
    public static string ToString(List<int> l)
    {
        StringBuilder result = new StringBuilder();
        result.Append(""["");
        for (int i = 0; i < l.Count; ++i)
        {
            if (i != 0) { result.Append("", ""); }
            result.Append(ToString(l[i]));
        }
        result.Append(""]"");
        return result.ToString();
    }
    public static string ToString(List<char> l)
    {
        StringBuilder result = new StringBuilder();
        result.Append(""["");
        for (int i = 0; i < l.Count; ++i)
        {
            if (i != 0) { result.Append("", ""); }
            result.Append(""'"");
            result.Append(ToString(l[i]));
            result.Append(""'"");
        }
        result.Append(""]"");
        return result.ToString();
    }
    public static string ToString(List<ValueTuple<int, object>> l)
    {
        StringBuilder result = new StringBuilder();
        result.Append(""["");
        for (int i = 0; i < l.Count; ++i)
        {
            if (i != 0) { result.Append("", ""); }
            result.Append(ToString(l[i]));
        }
        result.Append(""]"");
        return result.ToString();
    }
    public static void WriteLine(dynamic obj)
    {
        Console.WriteLine(ToString(obj));
    }
    public static void WriteLine()
    {
        Console.WriteLine();
    }
}";
        library.availableClasses["ConsoleExt"] = text;
    }
}