using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using py2cs;


public class AutoTests
{
    private void RunTests(string directory, string subDirectory, string mode)
    {
        string[] paths = Directory.GetFiles(Directory.GetCurrentDirectory());
        List<string> filenames = new List<string>();
        foreach (string path in paths)
        {
            string[] tokens = path.Split("/");
            string potentialFilename = tokens[tokens.Length - 1];
            if (potentialFilename.EndsWith(".py"))
            {
                // When importing, take only file that ends with "_0", which
                // is the main file.
                // It needs to end with "_0.py"

                // If it is the directory /import/* any file which does not end
                // with _0 is skipped.
                if (mode == "import" && potentialFilename.Length >= 5 &&
                    (potentialFilename[potentialFilename.Length - 5] != '_' ||
                    potentialFilename[potentialFilename.Length - 4] != '0'))
                {
                    continue;
                }
                // If it is in different directory (like 'unit'), any file which
                // ends with '_<char>' and char is not '0' is skipped.
                if (mode == "import" && potentialFilename.Length >= 5 &&
                    potentialFilename[potentialFilename.Length - 5] == '_' &&
                    potentialFilename[potentialFilename.Length - 4] != '0')
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
            string outputDirectory = output_path + directory;

            output_path += directory + "/" + name + ".cs";
            Translator translator = new Translator(true);
            // We are not dealing with imported files so we set the moduleName
            // to empty.
            if (translator.Translate(input_path, output_path, new List<string>(), mode))
            {
                translator.Compile(outputDirectory, name + ".cs");
            }
        }
    }

    [Fact]
    public void RunAllTests()
    {
        Directory.SetCurrentDirectory("../../../../tests/scripts/1_must_have");

        // ----Uncomment this block to run tests in scripts/must_have----
        // Directory.SetCurrentDirectory("../1_must_have");
        // RunTests("1_must_have", "", "");

        // ----Uncomment this block to run tests in scripts/should_have----
        // Directory.SetCurrentDirectory("../2_should_have");
        // RunTests("2_should_have", "", "");

        // ----Uncomment this block to run tests in scripts/nice_to_have----
         Directory.SetCurrentDirectory("../3_nice_to_have");
         RunTests("3_nice_to_have", "", "");

        // ----Uncomment this block to run tests in scripts/not_implemented----
        // Directory.SetCurrentDirectory("../4_not_implemented");
        // RunTests("4_not_implemented", "", "");

        // ----Uncomment this block to run tests in scripts/error----
         Directory.SetCurrentDirectory("../5_error");
         RunTests("5_error", "", "");

        // ----Uncomment this block to run tests in scripts/difference----
        // Directory.SetCurrentDirectory("../6_differences");
        // RunTests("6_differences", "", "differences");

        //  ----Uncomment this block to run tests in
        //  scripts/must_have/import
        //  scripts/must_have_input----
        /* Directory.SetCurrentDirectory("../1_must_have/import/1");
           RunTests("1_must_have/import/1", "import/1", "import");
           Directory.SetCurrentDirectory("../../");
           Directory.SetCurrentDirectory("../1_must_have/import/2");
           RunTests("1_must_have/import/2", "import/2", "import");
           Directory.SetCurrentDirectory("../../");
           Directory.SetCurrentDirectory("../1_must_have/import/3");
           RunTests("1_must_have/import/3", "import/3", "import");
           Directory.SetCurrentDirectory("../../");
           Directory.SetCurrentDirectory("../1_must_have/import/4");
           RunTests("1_must_have/import/4", "import/4", "import");
           Directory.SetCurrentDirectory("../../");
           Directory.SetCurrentDirectory("../1_must_have/input");
           RunTests("1_must_have/input", "input", "input"); */
    }
}