using System.Collections.Generic;
// This is a state for a scope (e.g. inside of if statement).
// It is used to manage the scope of declared variables;
public class ScopeState
{
    public HashSet<string> declaredIdentifiers;
    public bool isActive;
    public ScopeState()
    {
        declaredIdentifiers = new HashSet<string>();
        isActive = false;
    }
}