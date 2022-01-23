
L = [ t for t in enumerate(x for x in range(20) if x%3==0 or x%5==0) ]
print(L)
print()

i = len(L)
j = 0
k = -2

L1 = L[i:j:k]
print(L1)
L1[1] = (-1,-1)
print(L1)
print(L)
print()

L2 = L[::k]
print(L2)
L2[2] = (-1,-1)
print(L2)
print(L)
print()

L3 = L[i:i:k]
print(L3)
print(L)
