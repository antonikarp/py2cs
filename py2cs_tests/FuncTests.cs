﻿using System;
using Xunit;
using py2cs;

namespace py2cs_tests
{
    public class FuncTests
    {
        [Fact]
        public void Test1_VoidParameterless()
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
    public static void foo()
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
        public void Test2_NonVoidParameterless()
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
    public static object foo()
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
        [Fact]
        public void Test3_Parameters()
        {
            // Arrange
            string input =
@"def add(a, b):
    print(a + b)
";
            string expected =
@"using System;
class Program
{
    public static void add(dynamic a, dynamic b)
    {
        Console.WriteLine(a+b);
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
        public void Test3_Calls()
        {
            // Arrange
            string input =
@"def add(a, b):
	c = a + b
	print(c)
def hello():
	print(""Hello"")
add(3, 5)
hello()
";
            string expected =
@"using System;
class Program
{
    public static void add(dynamic a, dynamic b)
    {
        dynamic c = a+b;
        Console.WriteLine(c);
    }
    public static void hello()
    {
        Console.WriteLine(""Hello"");
    }
    static void Main(string[] args)
    {
        add(3, 5);
        hello();
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