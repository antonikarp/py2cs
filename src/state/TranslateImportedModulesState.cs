using System.Collections.Generic;
public class TranslateImportedModulesState
{
    public Dictionary<string, string> inputPathToOutputPath;
    public Dictionary<string, List<string>> inputPathToImportedFileNames;
    public TranslateImportedModulesState()
    {
        inputPathToOutputPath = new Dictionary<string, string>();
        inputPathToImportedFileNames = new Dictionary<string, List<string>>();
    }
}