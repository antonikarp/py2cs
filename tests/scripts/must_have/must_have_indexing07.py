
L = [ t for t in enumerate(x for x in range(20) if x%3==0 or x%5==0) ]
print(L)
print()

i = 6
j = 3
k = -1

LL = L[i::k]
print(LL)
LL[1] = (-1,-1)
print(LL)
print(L)
print()

LL = L[:j:k]
print(LL)
LL[1] = (-2,-2)
print(LL)
print(L)
print()

LL = L[i:j:k]
print(LL)
LL[1] = (-3,-3)
print(LL)
print(L)
print()

i = -2
j = -5

LL = L[i::k]
print(LL)
LL[1] = (-1,-1)
print(LL)
print(L)
print()

LL = L[:j:k]
print(LL)
LL[1] = (-2,-2)
print(LL)
print(L)
print()

LL = L[i:j:k]
print(LL)
LL[1] = (-3,-3)
print(LL)
print(L)
print()
