
class A(BaseException):
    pass

class B(A):
    pass

try:
    raise B()
except A:
    print('caught exception of type A')
except B:
    print('caught exception of type B')
except:
    print('caught any exception')

