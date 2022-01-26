x = 0

def fun1():

    def fun2():
        nonlocal x
        print(x)
        x = 2

    x = 1
    fun2()
    print(x)

fun1()
