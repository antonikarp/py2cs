
def fun1():

    def fun2():
        global x
        print(x)

    x = 1
    fun2()

fun1()
