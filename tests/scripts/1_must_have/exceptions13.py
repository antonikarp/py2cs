
def fun(n):
    try:
        x = 1/n
    finally:
        print('finally')
    return x

try:
    print(fun(1))
    print(fun(0))
    print(fun(-1))
except:
    print('caught')
