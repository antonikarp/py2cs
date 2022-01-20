
class A(BaseException):
    pass
class B(A):
    pass

try:
    try:
       raise B
    except B:
        print('caught exception of type B')
        raise
    except A:
        print('caught exception of type A - inner')
        raise
except A:
    print('caught exception of type A - outer')
