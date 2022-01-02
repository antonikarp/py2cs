public class IndentedLine
{
    public string line;

    // 0 - if on the same level of indentation as the previous line
    // +1 - if on the one level higher (example: statement after opening brace)
    // -1 - if on the one level lower (example: closing brace)
    public int increment;

    public IndentedLine(string _line, int _increment)
    {
        line = _line;
        increment = _increment;
    }

}