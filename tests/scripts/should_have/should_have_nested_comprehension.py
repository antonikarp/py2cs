for t in [ (a,b,c,d) for a in range(5) if a%2==0 for b in range(7) if b>a for c in range(2) for d in range(a) ]:
    print(t)
