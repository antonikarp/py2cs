using System;
class Program
{
    static void Main(string[] args)
    {
        dynamic a = 1;
        dynamic b = 2;
        dynamic c = 3;
        dynamic d = (a>b)||(b<c);
        dynamic e = (a>=b)&&(b<=c);
        dynamic f = (!(a>b))&&(!(b!=c));
    }
}
