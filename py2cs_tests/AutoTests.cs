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
                /*if (!potentialFilename.StartsWith("evaluation_order"))
                {
                    continue;
                }*/
                if (mode == "import" && !potentialFilename.StartsWith("main"))
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
        // Directory.SetCurrentDirectory("../5_error");
        // RunTests("5_error", "", "");

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