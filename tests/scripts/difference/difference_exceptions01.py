
class A:
    pass

try:
    raise A()
except:
    print('caught any exception')
