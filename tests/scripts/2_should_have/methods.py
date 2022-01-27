
class B:
    def setx(x):
        x.x = 'B'
    def fun(x):
        x.setx()
        print('class',x.x)

class D(B):
    def setx(x):
        x.x = 'D'
    def fun(x):
        x.setx()
        print('class',x.x)

B().fun()
D().fun()
