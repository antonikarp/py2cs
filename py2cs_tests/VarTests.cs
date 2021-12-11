using System;
using Xunit;
using py2cs;

namespace py2cs_tests
{
    public class VarTests
    {
        [Fact]
        public void Test1_List()
        {
            // Arrange
            string input =
@"a = [3, 4, 5]";
            string expected =
@"using System;
using System.Collections.Generic;
class Program
{
    static void Main(string[] args)
    {
        dynamic a = new List<object> {3, 4, 5};
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