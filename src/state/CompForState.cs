// CompForState is useful when composing dict comprehension where
// we need to remember the iterator variable. Consider:
//
// { 2*i:i**2 for i in range(10)}
// 
// Translated to:
// (from i in Enumerable.Range(0,10) select i).ToDictionary(i => 2*i, i => Math.Pow(i, 2))
//
// The iterator expression is "i"
public class CompForState
{
    public string iteratorExpr;
    public bool isActive;
    public CompForState()
    {
        isActive = false;
        iteratorExpr = "";
    }
}