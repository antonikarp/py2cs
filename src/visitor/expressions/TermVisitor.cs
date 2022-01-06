using Antlr4.Runtime.Misc;
using System.Collections.Generic;
// This is a visitor for a term, which can be composed of different
// factors and operators. For now, only the four basic arithmetic expressions are used:
// +, -, *, /
public class TermVisitor : Python3ParserBaseVisitor<LineModel>
{
    public LineModel result;
    public State state;
    // The output consists of factorVisitors interleaved with operands in a following way:
    // factorVisitors[i] + operands[i] + factorVisitors[i + 1] + operands[i + 1] ...
    public List<FactorVisitor> factorVisitors;
    public List<string> operands;
    public TermVisitor(State _state)
    {
        state = _state;
        factorVisitors = new List<FactorVisitor>();
        operands = new List<string>();
    }
    public void TranslateFloorDivision()
    /* x // y == Math.Floor((double) x/y ) */ 
    {
        for (int i = 0; i < operands.Count; ++i)
        {
            if (operands[i] == "//")
            {
                FactorVisitor x = factorVisitors[i];
                FactorVisitor y = factorVisitors[i + 1];
                // This is a dummy visitor.
                FactorVisitor replacement = new FactorVisitor(state);
                replacement.result = new LineModel();
                replacement.result.tokens.Add("Math.Floor(" + "(double)" +
                    x.result.ToString() + "/" + y.result.ToString() + ")");
                // Insert replacement directly after y.
                factorVisitors.Insert(i + 2, replacement);
                // Remove x and y.
                factorVisitors.RemoveAt(i);
                factorVisitors.RemoveAt(i);
                operands.RemoveAt(i);
                // Since we have invalidated the iterators, we go back to the beginning:
                i = -1;
            }

        }
    }

    public void TransformModuloDivision()
    // x mod y == x - (x // y) * y

    // The expression on the right hand side corresponds to:
    // factorVisitors[n]: x
    // operands[n]: "-("
    // factorVisitors[n + 1]: x
    // operands[n + 1]: "//"
    // factorVisitors[n + 2]: y
    // operands[n + 2]: ")*"
    // factorVisitors[n + 2]: y

    // The operation "//" will be translated by TranslateFloorDivision() 
    {
        int n = factorVisitors.Count;
        FactorVisitor x = factorVisitors[n - 2];
        FactorVisitor y = factorVisitors[n - 1];
        factorVisitors.RemoveAt(n - 1);
        operands.Add("-(");
        factorVisitors.Add(x);
        operands.Add("//");
        factorVisitors.Add(y);
        operands.Add(")*");
        factorVisitors.Add(y);
    }

    public void ProduceInterleavedOutput()
    {
        for (int i = 0; i < factorVisitors.Count; ++i)
        {
            if (i != 0)
            {
                result.tokens.Add(operands[i - 1]);
            }
            for (int j = 0; j < factorVisitors[i].result.tokens.Count; ++j)
            {
                result.tokens.Add(factorVisitors[i].result.tokens[j]);
            }
        }
    }

    public override LineModel VisitTerm([NotNull] Python3Parser.TermContext context)
    {
        result = new LineModel();
        // If there is one child then it is a factor.
        if (context.ChildCount == 1)
        {
            FactorVisitor newVisitor = new FactorVisitor(state);
            context.GetChild(0).Accept(newVisitor);
            for (int i = 0; i < newVisitor.result.tokens.Count; ++i)
            {
                result.tokens.Add(newVisitor.result.tokens[i]);
            }
        }
        // If there is more than one child then we have the following children:
        // Child #0: factor
        // Child #1: "*" or "/" or "%" or "//"
        // Child #2: factor
        // ...
        else if (context.ChildCount > 1)
        {
            // Expression is standalone:
            if (!state.stmtState.isLocked)
            {
                state.stmtState.isStandalone = true;
                state.stmtState.isLocked = true;
            }
            int n = context.ChildCount;
            FactorVisitor firstVisitor = new FactorVisitor(state);
            context.GetChild(0).Accept(firstVisitor);
            factorVisitors.Add(firstVisitor);
            
            int i = 1;
            while (i + 1 < n)
            {
                FactorVisitor newVisitor = new FactorVisitor(state);
                context.GetChild(i + 1).Accept(newVisitor);
                factorVisitors.Add(newVisitor);

                if (context.GetChild(i).ToString() == "*")
                {
                    operands.Add("*");
                }
                else if (context.GetChild(i).ToString() == "/")
                {
                    operands.Add("/");
                }
                else if (context.GetChild(i).ToString() == "//")
                {
                    operands.Add("//");
                }
                else if (context.GetChild(i).ToString() == "%")
                {
                    TransformModuloDivision();
                }

                i += 2;
            }
            TranslateFloorDivision();
            ProduceInterleavedOutput();
        }
        
        return result;
    }
    
}