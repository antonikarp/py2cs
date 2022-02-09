public class State
{
    public VarState varState;
    public StmtState stmtState;
    public LoopState loopState;
    public FuncCallState funcCallState;
    public CompForState compForState;
    public LhsTupleState lhsTupleState;
    public PromoteBoolToIntState promoteBoolToIntState;
    public ScopeState scopeState;
    public InputState inputState;
    public VarReferringToGlobalState varReferringToGlobalState;
    public FloatToIntConversionState floatToIntConversionState;
    public LhsState lhsState;
    public ListCompState listCompState;
    public TupleState tupleState;
    public ConstructorCallState constructorCallState;
    public ClassDefState classDefState;
    public TrailerConstructorCheckState trailerConstructorCheckState;
    public IllegalKeywordArgumentsState illegalKeywordArgumentsState;
    public TypeCastFromNullCheckState typeCastFromNullCheckState;
    public ExceptionAttributeState exceptionAttributeState;
    public TranslateImportedModulesState translateImportedModulesState;
    public Output output;
    public State()
    {
        varState = new VarState();
        stmtState = new StmtState();
        loopState = new LoopState();
        funcCallState = new FuncCallState();
        compForState = new CompForState();
        lhsTupleState = new LhsTupleState();
        promoteBoolToIntState = new PromoteBoolToIntState();
        scopeState = new ScopeState();
        inputState = new InputState();
        varReferringToGlobalState = new VarReferringToGlobalState();
        floatToIntConversionState = new FloatToIntConversionState();
        lhsState = new LhsState();
        listCompState = new ListCompState();
        tupleState = new TupleState();
        constructorCallState = new ConstructorCallState();
        classDefState = new ClassDefState();
        trailerConstructorCheckState = new TrailerConstructorCheckState();
        illegalKeywordArgumentsState = new IllegalKeywordArgumentsState();
        typeCastFromNullCheckState = new TypeCastFromNullCheckState();
        exceptionAttributeState = new ExceptionAttributeState();
        translateImportedModulesState = new TranslateImportedModulesState();
        output = new Output();
    }
}