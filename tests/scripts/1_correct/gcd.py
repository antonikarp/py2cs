def gcd(a,b):
    if a<=0 or b<=0:
        return -1
    while a!=b: 
        if a<b:
            a,b = b,a
        a -= b;
    return a

print(gcd(18,12))
print(gcd(12,18))
print(gcd(-18,12))
print(gcd(18,-12))
print(gcd(0,0))
