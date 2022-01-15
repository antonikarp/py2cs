using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using py2cs;


public class AutoTests
{
    private void RunTests(string directory)
    {
        string[] paths = Directory.GetFiles(Directory.GetCurrentDirectory());
        List<string> filenames = new List<string>();
        foreach (string path in paths)
        {
            string[] tokens = path.Split("/");
            string potentialFilename = tokens[tokens.Length - 1];
            if (potentialFilename.EndsWith(".py"))
            {
                // This is a temporary statement to reduce the number of test files.
                /*if (!potentialFilename.StartsWith("must_have_if_exp"))
                {
                    continue;
                }*/

                // For test files with imports, take only file that ends with "_0"
                // We exclude the other categories of tests like must_have_...
                if (potentialFilename.Contains("_") &&
                    potentialFilename.StartsWith("test") &&
                    !potentialFilename.EndsWith("_0.py"))
                {
                    continue;
                }
                string[] beforeDot = potentialFilename.Split(".");
                filenames.Add(beforeDot[0]);
            }
        }
        filenames.Sort();
        // Write all eligible filenames to a file which will be read by a bash scipt
        // "run_tests.sh"
        File.WriteAllLines("testnames.txt", filenames);
        foreach (string name in filenames)
        {
            string input_path = name + ".py";
            string output_path = "../../generated/" + directory + "/" + name + ".cs";
            Translator translator = new Translator();
            translator.Translate(input_path, output_path, "");
            translator.Compile(name + ".cs", directory);
        }
    }

    [Fact]
    public void RunAllTests()
    {
        Directory.SetCurrentDirectory("../../../../tests/scripts/unit");
        Directory.SetCurrentDirectory("../unit");
        RunTests("unit");
        Directory.SetCurrentDirectory("../must_have");
        RunTests("must_have");
    }
}