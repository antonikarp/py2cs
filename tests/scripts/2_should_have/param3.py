y = [1]
def fun(x=y):
    return x

print(fun(),y)
y[0] = 2
print(fun(),y)
print(fun(y),y)
