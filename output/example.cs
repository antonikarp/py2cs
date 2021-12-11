using System;
class Program
{
    static void Main(string[] args)
    {
        if (3>5)
        {
            Console.WriteLine("a");
            if (3>4)
            {
                Console.WriteLine("aa");
            }
            else
            {
                Console.WriteLine("ab");
            }
        }
        else if (4>5)
        {
            Console.WriteLine("b");
        }
        else
        {
            Console.WriteLine("c");
        }
    }
}
