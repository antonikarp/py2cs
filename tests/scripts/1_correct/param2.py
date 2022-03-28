y = 1
def fun(x=y):
    return x

print(fun(),y)
y = 2
print(fun(),y)
print(fun(y),y)
