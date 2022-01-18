
tab = [ 5, 8, -3, 10, 2, 5, 7, -6, 5, 1 ]

i = 1
while i<len(tab): 
    v = tab[i]
    j = i-1
    while j>=0 and v<tab[j]:
        tab[j+1] = tab[j]
        j -=1
    tab[j+1] = v
    i += 1

for i in range(len(tab)):
    print(tab[i])
