using System;
class Program
{
    static void Main(string[] args)
    {
        dynamic a = 5;
        while (a>0)
        {
            if (a==2)
            {
                Console.WriteLine(a);
            }
            else
            {
                Console.WriteLine(a+1);
            }
            a = a-1;
        }
    }
}
