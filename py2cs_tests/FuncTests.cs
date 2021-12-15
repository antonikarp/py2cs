using System;
using Xunit;
using py2cs;

namespace py2cs_tests
{
    public class FuncTests
    {
        [Fact]
        public void Test1_VoidFunction()
        {
            // Arrange
            string input =
@"def foo():
	print(""Hello"")
";
            string expected =
@"using System;
class Program
{
    public void foo()
    {
        Console.WriteLine(""Hello"");
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
        public void Test2_NonVoidFunction()
        {
            // Arrange
            string input =
@"def foo():
	a = 2
	return a
";
            string expected =
@"using System;
class Program
{
    public object foo()
    {
        dynamic a = 2;
        return a;
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