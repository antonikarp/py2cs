
class A(BaseException):
    pass

class B(BaseException):
    pass

try:
    raise A from B()
except A:
    print('caught exception of type A')

try:
    raise A() from A
except A:
    print('caught exception of type A')

try:
    raise A() from None
except A:
    print('caught exception of type A')
