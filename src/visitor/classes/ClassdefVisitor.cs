using System;
using Antlr4.Runtime.Misc;

public class ClassdefVisitor : Python3ParserBaseVisitor<Class>
{
    public Class result;
    public State state;
    public ClassdefVisitor(State _state)
    {
        state = _state;
    }
    public override Class VisitClassdef([NotNull] Python3Parser.ClassdefContext context)
    {
        state.classDefState = new ClassDefState();
        state.classDefState.isActive = true;
        // We assume that we have the following children

        // If no inheritance:

        // Child #0: "class"
        // Child #1: <class name>
        // Child #2: ":"
        // Child #3: suite

        // If simple inheritance (one parent class):

        // Child #0: "class"
        // Child #1: <class name>
        // Child #2: "("
        // Child #3: arglist
        // Child #4: ")"
        // Child #5: ":"
        // Child #6: suite

        result = new Class(state.output);
        result.name = context.GetChild(1).ToString();

        // Save the name of the class.
        state.output.allClassesNames.Add(result.name);
        state.output.namesToClasses[result.name] = result;

        state.output.currentClasses.Push(result);

        // Check if there is inhertitance (one parent class):
        if (context.arglist() != null)
        {
            TestVisitor parentNameVisitor = new TestVisitor(state);
            context.arglist().Accept(parentNameVisitor);
            string parentName = parentNameVisitor.result.ToString();
            // In C# it corresponds to Exception
            if (parentName == "BaseException")
            {
                Class exceptionClass = new Class(state.output);
                exceptionClass.name = "Exception";
                result.parentClass = exceptionClass;
            }
            else
            {
                result.parentClass = state.output.namesToClasses[parentName];
            }
        }

        SuiteVisitor suiteVisitor = new SuiteVisitor(state);
        context.suite().Accept(suiteVisitor);

        result = state.output.currentClasses.Pop();
        // If the function is not internal, add it to the list of classes in
        // the output.
        // Otherwise, add it to the list of classes in the current class.
        // Remember that at the bottom there is always the Program class.
        if (state.output.currentClasses.Count > 1)
        {
            Class parentClass = state.output.currentClasses.Peek();
            parentClass.internalClasses.Add(result);
        }
        else
        {
            state.output.classes.Add(result);
        }

        // Flush the ClassDefState.
        state.classDefState = new ClassDefState();
        return result;
    }

}
