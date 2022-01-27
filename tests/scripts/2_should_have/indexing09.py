L = [ [ 1, 2, 3 ],
      [ 4, 5, 6 ],
      [ 7, 8, 9 ] ]
print(L)
print()

L1 = L[1:]
print(L1)
print()

L12 = L[1:][:2]
print(L12)
print()

L122 = [ L2[:2] for L2 in L[1:] ]
print(L122)
print()

L122[0][0] = -1
print(L122)
print(L)
