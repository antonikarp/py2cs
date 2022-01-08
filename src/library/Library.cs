using System.Collections.Generic;
public class Library
{
    public Dictionary<string, Class> availableClasses;
    public HashSet<Class> classesToCommit;
    public Output output;
    public Library(Output _output)
    {
        classesToCommit = new HashSet<Class>();
        availableClasses = new Dictionary<string, Class>();
        output = _output;
        ConstructFormattedList();
    }
    public void ConstructFormattedList()
    {
        Class formattedList = new Class(output);
        formattedList = new Class(output);
        formattedList.name = "Generated_FormattedList";
        formattedList.libraryParentClassName = "List<dynamic>";

        Function toStringMethod = new Function(output);
        toStringMethod.name = "ToString";
        toStringMethod.isOverride = true;
        toStringMethod.overrideReturnTypeString = "string";
        toStringMethod.statements.lines.Add(new IndentedLine
            ("StringBuilder result = new StringBuilder();", 0));
        toStringMethod.statements.lines.Add(new IndentedLine
            ("result.Append(\"[\");", 0));
        toStringMethod.statements.lines.Add(new IndentedLine
            ("for (int i = 0; i < Count; ++i)", 0));
        toStringMethod.statements.lines.Add(new IndentedLine
            ("{", 1));
        toStringMethod.statements.lines.Add(new IndentedLine
            ("if (i != 0) { result.Append(\", \"); }", 0));
        toStringMethod.statements.lines.Add(new IndentedLine
            ("result.Append(this[i]);", -1));
        toStringMethod.statements.lines.Add(new IndentedLine
            ("}", 0));
        toStringMethod.statements.lines.Add(new IndentedLine
            ("result.Append(\"]\");", 0));
        toStringMethod.statements.lines.Add(new IndentedLine
            ("return result.ToString();", 0));
        formattedList.functions.Add(toStringMethod);

        // We are done with construction. Add the class to the dictionary.
        availableClasses["FormattedList"] = formattedList;
    }
    public void CommitFormattedList()
    {
        output.usingDirs.Add("System.Text");
        classesToCommit.Add(availableClasses["FormattedList"]);
    }
}