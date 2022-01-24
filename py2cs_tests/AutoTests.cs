using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using py2cs;


public class AutoTests
{
    private void RunTests(string directory, string subDirectory, bool isImport)
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
                if (potentialFilename.StartsWith("must_have_indexing"))
                {
                    continue;
                }
                if (!potentialFilename.StartsWith("must_have_exp_stmt"))
                {
                    continue;
                }
                // Todo: remove
                /*List<string> passingTests = new List<string>()
                {
                    "should_have_assignment3.py", "should_have_comprehension_dict.py",
                    "should_have_comprehension_list.py", "should_have_comprehension_set.py",
                    "should_have_enumerate.py", "should_have_exceptions07.py",
                    "should_have_exceptions09.py", "should_have_fibonacci_yield1.py",
                    "should_have_fibonacci_yield2.py", "should_have_fibonacci.py",
                    "should_have_global1.py", "should_have_global2.py",
                    "should_have_nonlocal.py", "should_have_scope2.py",
                    "should_have_tuple_assignment.py", "should_have_param2.py",
                    "should_have_param3.py", "should_have_param5.py"

                };
                if (!passingTests.Contains(potentialFilename))
                {
                    continue;
                }*/

                // When importing, take only file that ends with "_0", which
                // is the main file.
                // It needs to end with "_0.py"

                // If it is the directory /import/* any file which does not end
                // with _0 is skipped.
                if (isImport && potentialFilename.Length >= 5 &&
                    (potentialFilename[potentialFilename.Length - 5] != '_' ||
                    potentialFilename[potentialFilename.Length - 4] != '0'))
                {
                    continue;
                }
                // If it is in different directory (like 'unit'), any file which
                // ends with '_<char>' and char is not '0' is skipped.
                if (!isImport && potentialFilename.Length >= 5 &&
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
            output_path += directory + "/" + name + ".cs";
            Translator translator = new Translator();
            // We are not dealing with imported files so we set the moduleName
            // to empty.
            if (translator.Translate(input_path, output_path, new List<string>()))
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
        //RunTests("unit", "", false);
        Directory.SetCurrentDirectory("../must_have");
        RunTests("must_have", "", false);
        //Directory.SetCurrentDirectory("../should_have");
        //RunTests("should_have", "", false);
        /*Directory.SetCurrentDirectory("../difference");
        RunTests("difference", "");
        Directory.SetCurrentDirectory("../not_implemented");
        RunTests("not_implemented", "");
        Directory.SetCurrentDirectory("../error");
        RunTests("error", "");
        
        Directory.SetCurrentDirectory("../must_have/import/1");
        RunTests("must_have/import/1", "import/1", true);
        Directory.SetCurrentDirectory("../../");
        Directory.SetCurrentDirectory("../must_have/import/2");
        RunTests("must_have/import/2", "import/2", true);
        Directory.SetCurrentDirectory("../../");
        Directory.SetCurrentDirectory("../must_have/import/3");
        RunTests("must_have/import/3", "import/3", true);
        Directory.SetCurrentDirectory("../../");
        Directory.SetCurrentDirectory("../must_have/import/4");
        RunTests("must_have/import/4", "import/4", true);
        Directory.SetCurrentDirectory("../../");
        Directory.SetCurrentDirectory("../must_have/input");
        RunTests("must_have/input", "input", false);*/
    }
}