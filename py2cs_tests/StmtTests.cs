using System;
using Xunit;
using py2cs;

namespace py2cs_tests
{
    public class StmtTests
    {
        [Fact]
        public void Test1_NestedIf()
        {
            // Arrange
            string input =
@"if 3 > 5:
    print(""a"")
    if 3 > 4:
        print(""aa"")
    else:
        print(""ab"")
elif 4 > 5:
    print(""b"")
else:
	print(""c"")
";
            string expected =
@"using System;
class Program
{
    static void Main(string[] args)
    {
        if (3>5)
        {
            Console.WriteLine(""a"");
            if (3>4)
            {
                Console.WriteLine(""aa"");
            }
            else
            {
                Console.WriteLine(""ab"");
            }
        }
        else if (4>5)
        {
            Console.WriteLine(""b"");
        }
        else
        {
            Console.WriteLine(""c"");
        }
    }
}
";
            Translator translator = new Translator();
            // Act
            string actual = translator.Translate(input);
            // Assert
            Assert.Equal(expected, actual);

        }
        [Fact]
        public void Test2_NestedForLoop()
        {
            // Arrange
            string input =
@"a = [3, 4, 5]
b = [1, 2]
for x in a:
	for y in b:
		print(x + y)
";
            string expected =
@"using System;
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
";
            Translator translator = new Translator();
            // Act
            string actual = translator.Translate(input);
            // Assert
            Assert.Equal(expected, actual);

        }


    }
}