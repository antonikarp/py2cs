
def fun(n):
    try:
        if n<0:
            return
        print(1/n)
    except:
        print('caught exception of any type')
    else:
        print("_TEST_UNORDERED_OFF")
        print('no exception caught')
    finally:
        print('finally')

print('-----')
print("_TEST_UNORDERED_ON")
fun(1)
print('-----')
fun(0)
print('-----')
fun(-1)
print('-----')
