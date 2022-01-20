
class A(BaseException):
    def __init__(self,v):
        self.x = v

try:
    raise A(1)
except A as ae:
    print('caught exception of type A with attribute x =',ae.x)
