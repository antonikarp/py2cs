x = 0

def fun1():

    def fun2():
        nonlocal x
        print(x)

    x = 1
    fun2()

fun1()
