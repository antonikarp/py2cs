
class A:
    def info(self):
        print('class A')

class B:
    def info(self):
        print('class B')

ab = A()
ab.info()
ab.__class__= B
ab.info()
