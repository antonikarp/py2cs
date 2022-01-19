﻿using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using py2cs;


public class AutoTests
{
    private void RunTests(string directory, string subDirectory)
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
                /*if (potentialFilename != "unit21_0.py")
                {
                    continue;
                }*/

                // For test files with imports, take only file that ends with "_0", which
                // is the main file.
                // It needs to end with "_0.py"
                if (potentialFilename.Length >= 5 &&
                    (potentialFilename[potentialFilename.Length - 5] != '_') ||
                    (potentialFilename[potentialFilename.Length - 4] != '0'))
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
            string[] subDirectoryTokens = subDirectory.Split("/");

            string output_path = "../../";
            // Append additional "../" if we are deeper in the subdirectory.
            if (subDirectory != "")
            {
                for (int i = 0; i < subDirectoryTokens.Length; ++i)
                {
                    output_path += "../";
                }
            }
            output_path += "generated/";
            output_path += directory + "/" + name + ".cs";
            Translator translator = new Translator();
            // We are not dealing with imported files so we set the moduleName
            // to empty.
            if (translator.Translate(input_path, output_path, ""))
            {
                translator.Compile(name + ".cs", directory, subDirectory);
            }
        }
    }

    [Fact]
    public void RunAllTests()
    {
        Directory.SetCurrentDirectory("../../../../tests/scripts/unit");
        //Directory.SetCurrentDirectory("../unit");
        //RunTests("unit", "");
        /*Directory.SetCurrentDirectory("../must_have");
        RunTests("must_have", "");
        Directory.SetCurrentDirectory("../should_have");
        RunTests("should_have", "");
        Directory.SetCurrentDirectory("../difference");
        RunTests("difference", "");
        Directory.SetCurrentDirectory("../not_implemented");
        RunTests("not_implemented", "");
        Directory.SetCurrentDirectory("../error");
        RunTests("error", "");*/

        Directory.SetCurrentDirectory("../must_have/import/1");
        RunTests("must_have/import/1", "import/1");
        Directory.SetCurrentDirectory("../../");
        Directory.SetCurrentDirectory("../must_have/import/2");
        RunTests("must_have/import/2", "import/2");
    }
}