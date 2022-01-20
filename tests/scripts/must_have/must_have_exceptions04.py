
class A(BaseException):
    pass

class B(A):
    pass

try:
    raise B()
except B:
    print('caught exception of type B')
except A:
    print('caught exception of type A')
except:
    print('caught any exception')

