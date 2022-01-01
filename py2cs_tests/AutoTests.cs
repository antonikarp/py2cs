using System;
using System.Collections.Generic;
using Xunit;
using py2cs;

public class AutoTests
{
    [Fact]
    public void RunTests()
    {
        List<string> names = new List<string>
        {
            "test1", "test2", "test3",
            "test4", "test5", "test6",
            "test7", "test8", "test9",
            "test10", "test11", "test12",
            "test13", "test14", "test15",
            "test16"
        };
        foreach (string name in names)
        {
            string input_path = "../../../../tests/scripts/" + name + ".py";
            string output_path = "../../../../tests/generated/" + name + ".cs";
            Translator translator = new Translator();
            translator.Translate(input_path, output_path);
        }
    }
}