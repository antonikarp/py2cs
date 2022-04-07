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
    public static string ToString(double a)
    {
        return a.ToString(CultureInfo.InvariantCulture);
    }
    public static string ToString(object obj)
    {
        if (obj is string)
        {
            return ""'"" + obj.ToString() + ""'"";
        }
        else
        {
            return obj.ToString();
        }
    }
    public static string ToString(List<dynamic> list)
    {
        StringBuilder result = new StringBuilder();
        result.Append(""["");
        for (int i = 0; i < list.Count; ++i)
        {
            if (i != 0) { result.Append("", ""); }
            if (list[i] is null)
            {
                result.Append(""None"");
            }
            else
            {
                result.Append(ToString(list[i]));
            }
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
    public static string ToString(List<ValueTuple<object, object>> l)
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
    public static string ToString(HashSet<dynamic> s)
    {
        StringBuilder result = new StringBuilder();
        result.Append(""{"");
        int i = 0;
        foreach (var element in s)
        {
            if (i != 0) { result.Append("", ""); }
            result.Append(element is null ? ""None"" : ToString(element));
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
    public static string ToString(Dictionary<dynamic, dynamic> d)
    {
        StringBuilder result = new StringBuilder();
        result.Append(""{"");
        int i = 0;
        foreach (var kv in d)
        {
            if (i != 0) { result.Append("", ""); }
            result.Append(kv.Key is null ? ""None"" : ToString(kv.Key));
            result.Append("": "");
            result.Append(kv.Value is null ? ""None"" : ToString(kv.Value));
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
    public static void WriteLine(string obj1, double obj2)
    {
        Console.Write(obj1);
        Console.Write("" "");
        Console.Write(obj2.ToString(CultureInfo.InvariantCulture));
        Console.WriteLine();
    }
    public static void WriteLine(object obj1, object obj2, params object[] additional)
    {
        if (obj1 is string)
        {
            Console.Write(obj1);
        }
        else
        {
            Console.Write(ToString(obj1));
        }
        Console.Write("" "");
        if (obj2 is string)
        {
            Console.Write(obj2);
        }
        else
        {
            Console.Write(ToString(obj2));
        }
        for (int i = 0; i < additional.Length; ++i)
        {
            Console.Write("" "");
            if (additional[i] is string)
            {
                Console.Write(additional[i]);
            }
            else
            {
                Console.Write(ToString(additional[i]));
            }
        }
        Console.WriteLine();
    }
    public static string HandleTuple(ValueTuple<dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, ValueTuple<dynamic, dynamic>> tuple)
    {
        StringBuilder result = new StringBuilder();
        result.Append(""("");
        result.Append(tuple.Item1 is null ? ""None"" : ToString(tuple.Item1));
        result.Append("", "");
        result.Append(tuple.Item2 is null ? ""None"" : ToString(tuple.Item2));
        result.Append("", "");
        result.Append(tuple.Item3 is null ? ""None"" : ToString(tuple.Item3));
        result.Append("", "");
        result.Append(tuple.Item4 is null ? ""None"" : ToString(tuple.Item4));
        result.Append("", "");
        result.Append(tuple.Item5 is null ? ""None"" : ToString(tuple.Item5));
        result.Append("", "");
        result.Append(tuple.Item6 is null ? ""None"" : ToString(tuple.Item6));
        result.Append("", "");
        result.Append(tuple.Item7 is null ? ""None"" : ToString(tuple.Item7));
        result.Append("", "");
        result.Append(tuple.Item8 is null ? ""None"" : ToString(tuple.Item8));
        result.Append("", "");
        result.Append(tuple.Item9 is null ? ""None"" : ToString(tuple.Item9));
        result.Append("")"");
        return result.ToString();
    }

    public static void WriteLine(dynamic obj)
    {
        if (obj is null)
        {
            Console.WriteLine(""None"");
            return;
        }
        else if (obj is string)
        {
            Console.WriteLine(obj);
            return;
        }
        string typeString = obj.GetType().ToString();
        if (typeString.StartsWith(""System.ValueTuple`8"") && typeString.Contains(""System.ValueTuple`2""))
        {
            Console.WriteLine(HandleTuple((ValueTuple<dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, ValueTuple<dynamic, dynamic>>)obj));
        }
        else
        {
            Console.WriteLine(ToString(obj));
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