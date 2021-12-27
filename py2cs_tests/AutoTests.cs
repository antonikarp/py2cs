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
            "test1", "test2", "test3"
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