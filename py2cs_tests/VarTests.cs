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
        [Fact]
        public void Test2_Range()
        {
            // Arrange
            string input =
@"a = range(5 + 3)
b = range(1 + (5 * 2), 3 * 4)
c = range(1 * 4, 7 - (3 + 5), 2 + 1);";
            string expected =
@"using System;
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
";
            Translator translator = new Translator();
            // Act
            string actual = translator.Translate(input);
            // Assert
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void Test3_Dict()
        {
            // Arrange
            string input =
@"new_dict = {""a"": 1, ""b"": 2}
c = new_dict[""a""]";
            string expected =
@"using System;
using System.Collections.Generic;
class Program
{
    static void Main(string[] args)
    {
        dynamic new_dict = new Dictionary<object, object> {{""a"", 1}, {""b"", 2}};
        dynamic c = new_dict[""a""];
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