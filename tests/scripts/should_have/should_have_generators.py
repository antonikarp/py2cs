n = 0
for t in ( (a,b,c,d,e,f,g,h,i,j) for a in range(100000) for b in range(100000) for c in range(100000) for d in range(100000) for e in range(100000) for f in range(100000) for g in range(100000) for h in range(100000) for i in range(100000) for j in range(100000) ):
    print(t)
    n += 1
    if n==10:
        break
