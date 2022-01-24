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
    public static string ToString(HashSet<double> s)
    {
        StringBuilder result = new StringBuilder();
        result.Append(""{"");
        int i = 0;
        foreach (var element in s)
        {
            if (i != 0) { result.Append("", ""); }
            result.Append(ToString(element));
            ++i;
        }
        result.Append(""}"");
        return result.ToString();
    }
    public static string ToString(HashSet<char> s)
    {
        StringBuilder result = new StringBuilder();
        result.Append(""{"");
        int i = 0;
        foreach (var element in s)
        {
            if (i != 0) { result.Append("", ""); }
            result.Append(""'"");
            result.Append(ToString(element));
            result.Append(""'"");
            ++i;
        }
        result.Append(""}"");
        return result.ToString();
    }
    public static string ToString(Dictionary<int, double> d)
    {
        StringBuilder result = new StringBuilder();
        result.Append(""{"");
        int i = 0;
        foreach (var kv in d)
        {
            if (i != 0) { result.Append("", ""); }
            result.Append(ToString(kv.Key));
            result.Append("": "");
            result.Append(ToString(kv.Value));
            ++i;
        }
        result.Append(""}"");
        return result.ToString();
    }
    public static string ToString(Dictionary<object, object> d)
    {
        StringBuilder result = new StringBuilder();
        result.Append(""{"");
        int i = 0;
        foreach (var kv in d)
        {
            if (i != 0) { result.Append("", ""); }
            result.Append(ToString(kv.Key));
            result.Append("": "");
            result.Append(ToString(kv.Value));
            ++i;
        }
        result.Append(""}"");
        return result.ToString();
    }
    public static void WriteLine(List<dynamic> l1, List<dynamic> l2)
    {
        Console.Write(ToString(l1));
        Console.Write("" "");
        Console.Write(ToString(l2));
        Console.WriteLine();
    }
    public static void WriteLine(object obj1, object obj2, params object[] additional)
    {
        Console.Write(ToString(obj1));
        Console.Write("" "");
        Console.Write(ToString(obj2));
        for (int i = 0; i < additional.Length; ++i)
        {
            Console.Write("" "");
            Console.Write(ToString(additional[i]));
        }
        Console.WriteLine();
    }
    public static void WriteLine(dynamic obj)
    {
        if (obj != null)
        {
            Console.WriteLine(ToString(obj));
        }
        else
        {
            Console.WriteLine(""None"");
        }
    }
    public static void WriteLine()
    {
        Console.WriteLine();
    }
}";
        library.availableClasses["ConsoleExt"] = text;
    }
}