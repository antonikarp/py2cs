using System.Collections.Generic;
public class LoopState
{
    // This holds the name of the iteration variable. This variable cannot be assigned to, so
    // every ExprStmt with this variable on the lhs must be ignored.
    public string forStmtIterationVariable;

    // This indicates whether we have an else block.
    public bool hasElseBlock;

    public HashSet<string> declaredIdentifiers;

    public int generatedInBlockCount;
    public string nameForGeneratedVariable;

    public enum LoopType { ForLoop, WhileLoop, NoLoop }
    public LoopType loopType;
    public LoopState()
    {
        generatedInBlockCount = 0;
        nameForGeneratedVariable = "";
        forStmtIterationVariable = "";
        hasElseBlock = false;
        declaredIdentifiers = new HashSet<string>();
        loopType = LoopType.NoLoop;
    }

}