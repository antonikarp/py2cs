using System;
using Xunit;
using py2cs;

public class AutoTests
{
    [Fact]
    public void Test1_HelloWorld()
    {
        string input_path = "../../../../tests/scripts/test1.py";
        string output_path = "../../../../tests/generated/test1.cs";
        Test test = new Test();
        test.RunTest(input_path, output_path);
    }
}