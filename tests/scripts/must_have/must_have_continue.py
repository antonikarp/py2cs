for  i in range (20):
    if ( i%5==0 ):
        continue
    if ( i&1==0 ):
        print(i," is divisible by 2")
    if ( i%3==0 ):
        print(i," is divisible by 3")
