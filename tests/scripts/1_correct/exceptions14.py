
class A(BaseException):
    pass

def fun():
    raise A

try:
    fun()
except A:
    print('caught exception of type A')
