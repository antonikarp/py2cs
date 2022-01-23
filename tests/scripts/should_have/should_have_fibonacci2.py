fib = [0,1]
for i in range (2,10):
    f = fib[-1:-3:-1]
    fib.append(f[0]+f[1])
print( [ f for f in enumerate(fib) ] )
