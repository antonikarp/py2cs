public class LibOnceEnumerable
{
    public string text;
    public Library library;
    public LibOnceEnumerable(Library _library)
    {
        library = _library;
        text = @"
public class OnceEnumerable<T>: IEnumerable<T>
{
    private IEnumerator<T> enumerator;
    public OnceEnumerable(IEnumerator<T> _enumerator)
    {
        enumerator = _enumerator;
    }
    public IEnumerator<T> GetEnumerator()
    {
        return enumerator;
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return enumerator;
    }
}";
        library.availableClasses["OnceEnumerable"] = text;
    }
}