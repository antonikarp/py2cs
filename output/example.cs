using System;
using System.Linq;
class Program
{
    static void Main(string[] args)
    {
        dynamic a = Enumerable.Range(0,5+3);
        dynamic b = Enumerable.Range(1+(5*2),(3*4)-(1+(5*2)));
        dynamic c = Enumerable.Range(1*4,((7-(3+5))-(1*4))/(2+1)).Select(x => x*(2+1));
    }
}
