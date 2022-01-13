
class Base1:
    def Message(self):
        print("message from Base1")

class Base2:
    def Message(self):
        print("message from Base2")

class Derived(Base1,Base2):
    def Message(self):
        Base1.Message(self)
        Base2.Message(self)
        print("message from Derived")

d = Derived()
d.Message()

