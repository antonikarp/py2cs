public class LibListSlice
{
    public string text;
    public Library library;
    public LibListSlice(Library _library)
    {
        library = _library;
        text = @"
public static class ListSlice
{
    public static int AdjustEndpoint(int length, int endpoint, int stride)
    {
        if (endpoint < 0)
        {
            endpoint += length;
            if (endpoint < 0)
            {
                endpoint = stride < 0 ? -1 : 0;
            }
        }
        else if (endpoint >= length)
        {
            endpoint = stride < 0 ? length - 1 : length;
        }
        return endpoint;
    }
    public static List<dynamic> Get(dynamic list, int? start_nullable, int? stop_nullable, int? stride_nullable)
    {
        List<dynamic> result = new List<dynamic>();
        int n = list.Count;
        int stride = stride_nullable ?? 1;
        int start, stop;
        if (start_nullable == null)
        {
            start = stride < 0 ? n - 1 : 0;
        }
        else
        {
            start = AdjustEndpoint(n, (int)start_nullable, stride);
        }
        if (stop_nullable == null)
        {
            stop = stride < 0 ? -1 : n;
        }
        else
        {
            stop = AdjustEndpoint(n, (int)stop_nullable, stride);
        }
        int i = start;
        if (stride < 0)
        {
            while (i > stop)
            {
                result.Add(list[i]);
                i += stride;
            }
        }
        else
        {
            while (i < stop)
            {
                result.Add(list[i]);
                i += stride;
            }
        }
        return result;
    }
}";
        library.availableClasses["ListSlice"] = text;

    }
}