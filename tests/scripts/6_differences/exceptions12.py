
def fun(n):
    try:
        print(1/n)
    except:
        print('caught exception of any type')
    finally:
        print('finally')
        if n>=0:
            return
        print('n < 0')        

print('-----')
fun(1)
print('-----')
fun(0)
print('-----')
fun(-1)
print('-----')
