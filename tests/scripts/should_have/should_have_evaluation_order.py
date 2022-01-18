def fun(arg):
    print(arg)
    return arg

L = [0,0]
L[fun(0)],L[fun(1)] = fun(2),fun(3)
