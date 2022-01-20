
class A:
    def info(self):
        print('class A')

class B:
    def info(self):
        print('class B')

class C(A):
    def info(self):
        super().info()
        print('class C')

c = C()
c.info()
C.__bases__= (B,)
c.info()
