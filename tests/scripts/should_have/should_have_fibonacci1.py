prev = 0
print(0,prev)
curr = 1
print(1,curr)
for n in range (2,10):
    prev,curr = curr,prev+curr
    print(n,curr)
