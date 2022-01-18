print(not True or True)
print(not True | True)

print(not True and False)
print(not True & False)

def fun(arg):
    print(arg)
    return arg

print()
x = not fun(False) or fun(True)
print('wynik: ',x)
x = not fun(False) | fun(True)
print('wynik: ',x)

print()
x = not fun(True) and fun(False)
print('wynik: ',x)
x = not fun(True) & fun(False)
print('wynik: ',x)
