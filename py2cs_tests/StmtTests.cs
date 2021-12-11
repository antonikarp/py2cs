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
    }
}