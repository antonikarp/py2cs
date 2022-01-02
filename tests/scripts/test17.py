def add(a, b):
    def add1(a):
        return a + 1
    for i in range(b):
        a = add1(a)
    return a
print(add(2, 3))