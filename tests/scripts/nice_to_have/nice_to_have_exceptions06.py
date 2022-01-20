
class A(BaseException):
    pass

try:
    raise A
except A:
    print('caught exception of type A')
except A:
    print('also caught exception of type A')
except:
    print('caught any exception')

