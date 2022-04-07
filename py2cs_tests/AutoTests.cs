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
                /*if (!potentialFilename.StartsWith("literals"))
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
        Directory.SetCurrentDirectory("../../../../tests/scripts/1_correct");

        // ----Uncomment this block to run tests in scripts/1_correct----
         Directory.SetCurrentDirectory("../1_correct");
         RunTests("1_correct", "", "");

        // ----Uncomment this block to run tests in scripts/2_not_implemented----
        // Directory.SetCurrentDirectory("../2_not_implemented");
        // RunTests("2_not_implemented", "", "");

        // ----Uncomment this block to run tests in scripts/3_incorrect_scripts----
        // Directory.SetCurrentDirectory("../3_incorrect_scripts");
        // RunTests("3_incorrect_scripts", "", "");

        // ----Uncomment this block to run tests in scripts/4_differences----
        // Directory.SetCurrentDirectory("../4_differences");
        // RunTests("4_differences", "", "differences");

        //  ----Uncomment this block to run tests in
        //  scripts/1_correct/import
        //  scripts/1_correct/input----
         Directory.SetCurrentDirectory("../1_correct/import/1");
         RunTests("1_correct/import/1", "import/1", "import");
         Directory.SetCurrentDirectory("../../");
         Directory.SetCurrentDirectory("../1_correct/import/2");
         RunTests("1_correct/import/2", "import/2", "import");
         Directory.SetCurrentDirectory("../../");
         Directory.SetCurrentDirectory("../1_correct/import/3");
         RunTests("1_correct/import/3", "import/3", "import");
         Directory.SetCurrentDirectory("../../");
         Directory.SetCurrentDirectory("../1_correct/import/4");
         RunTests("1_correct/import/4", "import/4", "import");
         Directory.SetCurrentDirectory("../../");
         Directory.SetCurrentDirectory("../1_correct/import/5");
         RunTests("1_correct/import/5", "import/5", "import");
         Directory.SetCurrentDirectory("../../"); 
         Directory.SetCurrentDirectory("../1_correct/input");
         RunTests("1_correct/input", "input", "input");
    }
}