
def repeat(n=3):
    def helperdecorator(func):
        def wrapped(arg):
            for _ in range(n):
                result = func(arg)
            return result
        return wrapped
    return helperdecorator

@repeat()
def myfunction(arg):
    print('inside myfunction: ',arg)

myfunction(5)
