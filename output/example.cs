using System;
using System.Collections.Generic;
class Program
{
    static void Main(string[] args)
    {
        dynamic a = new List<object> {3, 4, 5};
        dynamic b = new List<object> {1, 2};
        foreach (dynamic x in a)
        {
            foreach (dynamic y in b)
            {
                Console.WriteLine(x+y);
            }
        }
    }
}
