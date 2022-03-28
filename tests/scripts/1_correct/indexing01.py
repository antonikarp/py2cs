
L = [ t for t in enumerate(x for x in range(20) if x%3==0 or x%5==0) ]
print(L)
print()

i = 0
j = len(L)

L1 = L[i:j]
print(L1)
L1[1] = (-1,-1)
print(L1)
print(L)
print()

L2 = L[:]
print(L2)
L2[2] = (-1,-1)
print(L2)
print(L)
print()

L3 = L[::]
print(L3)
L3[3] = (-1,-1)
print(L3)
print(L)
print()

L4 = L[i:-1]
print(L4)
L4[4] = (-1,-1)
print(L4)
print(L)
print()

L5 = L[i:i]
print(L5)
print(L)
