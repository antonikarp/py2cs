
class A(BaseException):
    pass

try:
    raise A
except A:
    print('caught exception of type A')

try:
    raise A()
except A:
    print('caught exception of type A')
