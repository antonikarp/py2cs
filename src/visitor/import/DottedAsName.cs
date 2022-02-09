using System;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;
// This is a visitor used to process an individual import (possibly with an alias) or a series of imports.
public class DottedAsNameVisitor : Python3ParserBaseVisitor<Empty>
{
    public Empty result;
    public State state;
    public DottedAsNameVisitor(State _state)
    {
        state = _state;
    }
    public override Empty VisitDotted_as_name([NotNull] Python3Parser.Dotted_as_nameContext context)
    {
        result = new Empty();
        
        string filename = "";
        string name = "";
        DottedNameVisitor newVisitor = new DottedNameVisitor(state);
        context.dotted_name().Accept(newVisitor);
        filename = newVisitor.result.value;

        if (context.AS() == null)
        {
            // Here we process a simple import statement: "import <module>"
            // We have only one child: dotted_name
            // Name of the module is the same as the filename. 
            name = filename;
        }
        else
        {
            // We have the following children:
            // Child #0: dotted_name
            // Child #1: "as"
            // Child #2: <alias>
            name = context.GetChild(2).ToString();
        }
        string input_path;
        string output_path;
        if (py2cs.Program.input_path != null && py2cs.Program.output_path != null)
        {
            input_path = py2cs.Program.input_path;
            output_path = py2cs.Program.output_path;
        }
        else
        {
            input_path = py2cs.Translator.input_path;
            output_path = py2cs.Translator.output_path;
        }
        string[] tokens1 = input_path.Split("/");
        string[] tokens2 = output_path.Split("/");
        string new_input_path = "";
        for (int i = 0; i < tokens1.Length - 1; ++i)
        {
            new_input_path += tokens1[i];
            new_input_path += "/";
        }
        new_input_path += filename;
        new_input_path += ".py";

        string new_output_path = "";
        for (int i = 0; i < tokens2.Length - 1; ++i)
        {
            new_output_path += tokens2[i];
            new_output_path += "/";
        }
        new_output_path += filename;
        new_output_path += ".cs";

        // Explicitly set new paths which contain the imported filename at the end.
        // Add the name to the importedFilenames.
        List<string> importedFileNamesCurrent = new List<string>(state.output.moduleNames);
        importedFileNamesCurrent.Add(name);

        // Save the arguments that will be used at the end of parsing to invoke the translator.
        state.translateImportedModulesState.inputPathToOutputPath[new_input_path] = new_output_path;
        state.translateImportedModulesState.inputPathToImportedFileNames[new_input_path] = importedFileNamesCurrent;

        // Save the alias only if hasn't been saved before.
        if (!state.output.usedNamesFromImport.ContainsKey(filename))
        {
            state.output.usedNamesFromImport[filename] = new List<string>();
        }
        if (!state.output.usedNamesFromImport[filename].Contains(name))
        {
            state.output.usedNamesFromImport[filename].Add(name);
        }
        return result;
    }


}