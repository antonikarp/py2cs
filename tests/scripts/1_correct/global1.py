
print('--- fun 1 ---')

def fun1():
    print(x1)

x1 = 1
fun1()
print(x1)

print('--- fun 2 ---')

def fun2():
    global x2, y
    print(x2)
    x2 = 22

x2 = 2
fun2()
print(x2)
