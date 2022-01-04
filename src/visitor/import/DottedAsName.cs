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
        // So far, we process an individual import without any alias.
        if (context.dotted_name() != null)
        {
            DottedNameVisitor newVisitor = new DottedNameVisitor(state);
            context.dotted_name().Accept(newVisitor);
            string name = newVisitor.result.value;
            py2cs.Translator translator = new py2cs.Translator();
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
            new_input_path += name;
            new_input_path += ".py";

            string new_output_path = "";
            for (int i = 0; i < tokens2.Length - 1; ++i)
            {
                new_output_path += tokens2[i];
                new_output_path += "/";
            }
            new_output_path += name;
            new_output_path += ".cs";

            translator.Translate(new_input_path, new_output_path, name);
            // Add filenames as command line arguments for compilation.
            string commandLineArgument = name + ".cs";
            py2cs.Translator.importedFilenames.Add(commandLineArgument);
            List<string> allClassesFromModule = translator.outputVisitor.state.output.allClasses;
            for (int i = 0; i < allClassesFromModule.Count; ++i)
            {
                if (!state.output.allClasses.Contains(allClassesFromModule[i]))
                {
                    state.output.allClasses.Add(allClassesFromModule[i]);
                }
            }
        }
        return result;
    }


}