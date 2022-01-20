
class A(BaseException):
    pass
class B(BaseException):
    pass
class C(BaseException):
    pass

try:
    raise B()
except A:
    print('caught exception of type A')
except (B,C):
    print('caught exception of type B or C')
