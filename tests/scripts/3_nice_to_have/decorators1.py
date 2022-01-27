
def mydecorator(func):
    def wrapped(arg):
        print('before call')
        result = func(arg)
        print('after call')
        return result
    return wrapped

def myfunction1(arg):
    print('inside myfunction1: ',arg)

@mydecorator
def myfunction2(arg):
    print('inside myfunction2: ',arg)

myfunction22 = myfunction2

myfunction1(3)
print("###")
myfunction2(3)
print("###")
myfunction22(3)
print("###")
