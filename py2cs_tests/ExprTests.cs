using System;
using Xunit;
using py2cs;

namespace py2cs_tests
{
    public class ExprTests
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

        [Fact]
        public void Test3_UnaryOps()
        {
            // Arrange
            string input =
@"var = +3
var2 = -(-(var))
";
            string expected =
@"using System;
class Program
{
    static void Main(string[] args)
    {
        dynamic var = +3;
        dynamic var2 = -(-(var));
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
        public void Test4_ModAndFloor()
        {
            // Arrange
            string input =
@"var = (3 // 5) + (2 // 4) + (5 % 2)
";
            string expected =
@"using System;
class Program
{
    static void Main(string[] args)
    {
        dynamic var = (Math.Floor(3/5))+(Math.Floor(2/4))+(5%2);
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
        public void Test5_Shift()
        {
            // Arrange
            string input =
@"var1 = (1 >> 2) + (3 << 4)
var2 = var1 >> 1
";
            string expected =
@"using System;
class Program
{
    static void Main(string[] args)
    {
        dynamic var1 = (1>>2)+(3<<4);
        dynamic var2 = var1>>1;
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
