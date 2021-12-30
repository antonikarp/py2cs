# Generators.

def foo():
    for i in range(5):
        yield 2*i

a = foo()
for x in a:
    print(x)