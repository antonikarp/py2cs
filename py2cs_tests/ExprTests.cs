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


        [Fact]
        public void Test6_CompOp()
        {
            // Arrange
            string input =
@"a = (3 != 5)
b = (4 < 2)
c = (1 > 0)
d = (0 == 0)
e = (1 >= 2)
f = (0 <= 1)
";
            string expected =
@"using System;
class Program
{
    static void Main(string[] args)
    {
        dynamic a = (3!=5);
        dynamic b = (4<2);
        dynamic c = (1>0);
        dynamic d = (0==0);
        dynamic e = (1>=2);
        dynamic f = (0<=1);
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
        public void Test7_LogicalOp()
        {
            // Arrange
            string input =
@"a = 1
b = 2
c = 3
d = (a > b) or (b < c)
e = (a >= b) and (b <= c)
f = (not(a > b)) and (not(b != c))
";
            string expected =
@"using System;
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
";
            Translator translator = new Translator();
            // Act
            string actual = translator.Translate(input);
            // Assert
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void Test8_ChainComp()
        {
            // Arrange
            string input =
@"a = (2 < 3 < 4)
b = (2 > 3 < 4 > 5 <= 6)
";
            string expected =
@"using System;
class Program
{
    static void Main(string[] args)
    {
        dynamic a = ((2<3)&&(3<4));
        dynamic b = ((2>3)&&(3<4)&&(4>5)&&(5<=6));
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
        public void Test9_Assignment()
        {
            // Arrange
            string input =
@"a = 3
a = 5
";
            string expected =
@"using System;
class Program
{
    static void Main(string[] args)
    {
        dynamic a = 3;
        a = 5;
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
        public void Test10_TernaryOperator()
        {
            // Arrange
            string input =
@"x = 2 if True else 3
x = 3 if False else 4
";
            string expected =
@"using System;
class Program
{
    static void Main(string[] args)
    {
        dynamic x = true ? 2 : 3;
        x = false ? 3 : 4;
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
        public void Test11_AugAssign()
        {
            // Arrange
            string input =
@"a = 1
a += 2
a -= 3
a *= 4
a /= 5
a %= 6
";
            string expected =
@"using System;
class Program
{
    static void Main(string[] args)
    {
        dynamic a = 1;
        a += 2;
        a -= 3;
        a *= 4;
        a /= 5;
        a %= 6;
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
        public void Test12_PowerOperator()
        {
            // Arrange
            string input =
@"a = 2
b = a ** (a ** 2)
";
            string expected =
@"using System;
class Program
{
    static void Main(string[] args)
    {
        dynamic a = 2;
        dynamic b = Math.Pow(a, (Math.Pow(a, 2)));
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
