def Fibonacci():
    f0 = 0
    f1 = 1
    while True:
        fn = f0+f1
        f0 = f1
        f1 = fn
        yield fn

fib = Fibonacci()
for f in fib:
    print(f)
    if ( f>10 ):
        break
print("#####")
for f in fib:
    print(f)
    if ( f>100 ):
        break
print("#####")
for f in fib:
    print(f)
    if ( f>1000 ):
        break
