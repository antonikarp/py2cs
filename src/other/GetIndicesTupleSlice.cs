using System.Collections.Generic;
public static class GetIndicesTupleSlice
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
    public static List<int> Get(int n, int? start_nullable, int? stop_nullable, int? stride_nullable)
    {
        List<int> result = new List<int>();
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
                result.Add(i);
                i += stride;
            }
        }
        else
        {
            while (i < stop)
            {
                result.Add(i);
                i += stride;
            }
        }
        return result;
    }


}