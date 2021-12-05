using System;
using Xunit;
using py2cs;

namespace py2cs_tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1_HelloWorld()
        {
            // Arrange
            string input =
@"print(""Hello world"")
";
            string expected =
@"using System;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(""Hello world"");
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
        public void Test2_VarDeclAndPrint()
        {
            // Arrange
            string input =
@"var = (2+2)*2-4/(2+3)
print(var)
";
            string expected =
@"using System;
class Program
{
    static void Main(string[] args)
    {
        dynamic var = (2+2)*2-4/(2+3);
        Console.WriteLine(var);
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
