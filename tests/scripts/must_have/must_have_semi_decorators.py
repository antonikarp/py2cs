
def repeat(n=3):
    def helperdecorator(func):
        def wrapped(arg):
            for _ in range(n):
                result = func(arg)
            return result
        return wrapped
    return helperdecorator

def myfunction(arg):
    print('inside myfunction: ',arg)

repeat(4)(myfunction)(7)