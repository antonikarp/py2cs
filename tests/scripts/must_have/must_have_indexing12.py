L = [ 2, 3, 5, 7, 11, 13, 17, 19 ]
i = L[0]
j = L[1]
k = L[2]
print(L[i*j-k:j*k-i:L[1]])
print(L[j*k-i:i*j-k:-L[0]])

