using Antlr4.Runtime.Misc;
using System;

// This visitor is used in translating:
// * function calls
// * subscriptions (ex. dict_a["key"])

public class TrailerVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    public TrailerVisitor(State _state)
    {
        state = _state;
    }

    public override LineModel VisitTrailer([NotNull] Python3Parser.TrailerContext context)
    {
        result = new LineModel();
        // Function call - some parameters
        if (context.ChildCount == 3 && context.GetChild(0).ToString() == "(" &&
            context.GetChild(2).ToString() == ")" &&
            context.GetChild(1).GetType().ToString() == "Python3Parser+ArglistContext")
        {
            result.tokens.Add("(");
            // We assume that we have the following children:
            // Child #0: argument_0
            // Child #1: ","
            // Child #2: argument_1
            // ...
            int n = context.GetChild(1).ChildCount;
            int i = 0;
            while (i < n)
            {
                if (i != 0)
                {
                    result.tokens.Add(", ");
                }
                ArgumentVisitor newVisitor = new ArgumentVisitor(state);
                context.GetChild(1).GetChild(i).Accept(newVisitor);
                for (int j = 0; j < newVisitor.result.tokens.Count; ++j)
                {
                    result.tokens.Add(newVisitor.result.tokens[j]);
                }
                i += 2;
            }
            result.tokens.Add(")");
            // Case of enumerate. We use a Linq query to create a list of tuples.
            if (state.funcCallState.funcName == "enumerate")
            {
                state.output.usingDirs.Add("System.Linq");
                result.tokens.Add(".Select((p1, p2) => ValueTuple.Create(p2, p1))");
                // We are done. Flush the FuncCall state.
                state.funcCallState = new FuncCallState();
            }
        }
        // Subscription and slices
        else if (context.ChildCount == 3 && context.GetChild(0).ToString() == "[" &&
            context.GetChild(2).ToString() == "]")
        {

            Subscript_Visitor newVisitor = new Subscript_Visitor(state);
            context.GetChild(1).Accept(newVisitor);
            string value = newVisitor.result.ToString();
            string[] parts = value.Split(':');
            bool includeLinq = true;

            // We have the case of a subscription: (e.g. a[1])
            if (parts.Length == 1)
            {
                includeLinq = false;
                result.tokens.Add("[");
                result.tokens.Add(value);
                result.tokens.Add("]");
            }
            // -------
            // Case 1. a[:]
            // Result: The whole array.
            else if (parts.Length == 2 && parts[0].Length == 0 && parts[1].Length == 0)
            {
                includeLinq = false;
            }

            // Case 2. a[low:] = [a[low], a[low+1], ..., a[len(a)-1]]
            // Result: a.Skip(low);
            else if (parts.Length == 2 && parts[0].Length != 0 && parts[1].Length == 0)
            {
                result.tokens.Add(".Skip(");
                result.tokens.Add(parts[0]);
                result.tokens.Add(")");
            }

            // Case 3. a[:high] = [a[0], a[1], ..., a[high-1]]
            // Result: a.Take(high);
            else if (parts.Length == 2 && parts[0].Length == 0 && parts[1].Length != 0)
            {
                result.tokens.Add(".Take(");
                result.tokens.Add(parts[1]);
                result.tokens.Add(")");
            }

            // Case 4. a[low:high] = [a[low], a[low+1], ..., a[high-1]]
            // Result: a.Skip(low).Take((high) - (low));
            else if (parts.Length == 2 && parts[0].Length != 0 && parts[1].Length != 0)
            {
                result.tokens.Add(".Skip(");
                result.tokens.Add(parts[0]);
                result.tokens.Add(").Take((");
                result.tokens.Add(parts[1]);
                result.tokens.Add(")-(");
                result.tokens.Add(parts[0]);
                result.tokens.Add("))");
            }

            // -------
            // stride > 0
            // -------
            // Case 5. a[::stride] = Case 1 (every n=stride)
            // Result: a.Where((x, i) => (i % (stride) == 0));
            else if (parts.Length == 3 && parts[0].Length == 0
                && parts[1].Length == 0 && parts[2].Length != 0 &&
                Int32.Parse(parts[2]) > 0)
            {
                result.tokens.Add(".Where((x, i) => (i % (");
                result.tokens.Add(parts[2]);
                result.tokens.Add(") == 0))");
            }

            // Case 6. a[low::stride] = Case 2 (every n=stride)
            // Result: a.Skip(low).Where((x, i) => (i % (stride) == 0));
            else if (parts.Length == 3 && parts[0].Length != 0
                && parts[1].Length == 0 && parts[2].Length != 0 &&
                Int32.Parse(parts[2]) > 0)
            {
                result.tokens.Add(".Skip(");
                result.tokens.Add(parts[0]);
                result.tokens.Add(")");
                result.tokens.Add(".Where((x, i) => (i % (");
                result.tokens.Add(parts[2]);
                result.tokens.Add(") == 0))");
            }

            // Case 7: a[:high:stride] = Case 3 (every n=stride)
            // Result: a.Take(high).Where((x, i) => (i % (stride) == 0));
            else if (parts.Length == 3 && parts[0].Length == 0
                && parts[1].Length != 0 && parts[2].Length != 0 &&
                Int32.Parse(parts[2]) > 0)
            {
                result.tokens.Add(".Take(");
                result.tokens.Add(parts[1]);
                result.tokens.Add(")");
                result.tokens.Add(".Where((x, i) => (i % (");
                result.tokens.Add(parts[2]);
                result.tokens.Add(") == 0))");
            }

            // Case 8: a[low:high:stride] = Case 4 (every n=stride)
            // Result: a.Skip(low).Take((high) - (low)).Where((x, i) => (i % (stride) == 0));
            else if (parts.Length == 3 && parts[0].Length != 0 &&
                parts[1].Length != 0 && parts[2].Length != 0 &&
                Int32.Parse(parts[2]) > 0)
            {
                result.tokens.Add(".Skip(");
                result.tokens.Add(parts[0]);
                result.tokens.Add(").Take((");
                result.tokens.Add(parts[1]);
                result.tokens.Add(")-(");
                result.tokens.Add(parts[0]);
                result.tokens.Add("))");
                result.tokens.Add(".Where((x, i) => (i % (");
                result.tokens.Add(parts[2]);
                result.tokens.Add(") == 0))");
            }

            // -------
            // stride == -1
            // -------
            // Note: negative values of strides are so far handled only for -1.

            // Case 9: a[::-1] = Case 1.Reverse()
            // Result: a.AsEnumerable().Reverse().ToList()
            else if (parts.Length == 3 && parts[0].Length == 0 &&
                parts[1].Length == 0 && parts[2].Length != 0 &&
                Int32.Parse(parts[2]) == -1)
            {
                result.tokens.Add(".AsEnumerable().Reverse().ToList()");
            }

            // Case 10: a[high::-1] = Case 2.Reverse()
            // Result: a.Take((high)+1).AsEnumerable().Reverse().ToList()
            else if (parts.Length == 3 && parts[0].Length != 0 &&
                parts[1].Length == 0 && parts[2].Length != 0 &&
                Int32.Parse(parts[2]) == -1)
            {
                result.tokens.Add(".Take((");
                result.tokens.Add(parts[0]);
                result.tokens.Add(")+1)");
                result.tokens.Add(".AsEnumerable().Reverse().ToList()");
            }

            // Case 11: a[:low:-1] = Case 3.Reverse()
            // Result: a.Skip((low)+1).AsEnumerable().Reverse().ToList()
            else if (parts.Length == 3 && parts[0].Length == 0 &&
                parts[1].Length != 0 && parts[2].Length != 0 &&
                Int32.Parse(parts[2]) == -1)
            {
                result.tokens.Add(".Skip((");
                result.tokens.Add(parts[1]);
                result.tokens.Add(")+1)");
                result.tokens.Add(".AsEnumerable().Reverse().ToList()");
            }

            // Case 12: a[high:low:-1] = Case 4.Reverse()
            // Result: a.Skip((low)+1).Take((high)-1).AsEnumerable().Reverse().ToList()
            else if (parts.Length == 3 && parts[0].Length != 0 &&
                parts[1].Length != 0 && parts[2].Length != 0 &&
                Int32.Parse(parts[2]) == -1)
            {
                result.tokens.Add(".Skip((");
                result.tokens.Add(parts[1]);
                result.tokens.Add(")+1)");
                result.tokens.Add(".Take((");
                result.tokens.Add(parts[0]);
                result.tokens.Add(")-1)");
                result.tokens.Add(".AsEnumerable().Reverse().ToList()");
            }


            if (includeLinq)
            {
                state.output.usingDirs.Add("System.Linq");
            }
        }
        // Function call - no parameters
        else if (context.ChildCount == 2 && context.GetChild(0).ToString() == "(" &&
            context.GetChild(1).ToString() == ")")
        {
            result.tokens.Add("()");
        }
        // Field (ex. self.name)
        else if (context.ChildCount == 2 && context.GetChild(0).ToString() == ".")
        {
            result.tokens.Add(".");
            string name = context.GetChild(1).ToString();

            // Add a line to the field declarations.
            if (!state.output.currentClasses.Peek().fields.Contains(name))
            {
                string line = "public dynamic ";
                line += name;
                line += ";";
                state.output.currentClasses.Peek().fieldDecl.lines.Add(new IndentedLine(line, 0));

                // Add a name to the fields of the class.
                state.output.currentClasses.Peek().fields.Add(name);
            }

            result.tokens.Add(name);
        }

        return result;
    }

}