
x = 1

def fun():
    print(x)
    global x
    x = 11

print(x)
fun()
print(x)
