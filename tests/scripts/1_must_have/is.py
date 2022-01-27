L1 = [1,2]
L2 = [1,2]
print(L1==L2)
print(L1 is L2)
print()

a = 44
b = 44
c = 40 + 4
print(a==b)
print(a is b)
print(a is c)
print()

s1 = 'text'
s2 = "text"
print(s1==s2)
print(s1 is s2)
print()

class A:
    pass
a1 = A()
a2 = A()
print(a1==a2)
print(a1 is a2)
print()

class B:
    def __init__(self,v):
        self.val = v
b1 = B(1)
b2 = B(1)
print(b1==b2)
print(b1 is b2)
