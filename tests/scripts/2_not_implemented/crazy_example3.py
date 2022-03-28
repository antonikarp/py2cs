
class A:
    def info(self):
        print('class A - first version')

a1 = A()
a1.info()

class A:
    def info(self):
        print('class A - second version')

a2 = A()
a2.info()

a1.info()
