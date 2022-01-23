def fun(arg):
    print(arg)
    return arg

L = [0]
L[fun(0)] = fun(1)
