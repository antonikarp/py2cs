
class A:
    x = 1 
    pass

a1 = A()
a2 = A()
A.y = 11
print(A.x,A.y)
print(a1.x,a1.y)
print(a2.x,a2.y)
a2.x = 2
print(A.x)
print(a1.x)
print(a2.x)
A.x = 3
print(A.x)
print(a1.x)
print(a2.x)
