x = 0

def fun1():
    x = 1
    fun2()

def fun2():
    nonlocal x
    print(x)

fun1()
