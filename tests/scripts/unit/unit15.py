a = [x for x in range(10)]

# Case 1: seq[:] - copy of the whole array
print("Case 1:")
for y in a[:]:
    print(y)
    
# Case 2: seq[low:] = [seq[low], seq[low+1], ..., seq[len(a)-1]]
print("Case 2:")
for y in a[1:]:
    print(y)
    
# Case 3: seq[:high] = [seq[0], seq[1], ..., seq[high-1]]
print("Case 3:")
for y in a[:2]:
    print(y)
    
# Case 4: seq[low:high] = [seq[low], seq[low+1], ..., seq[high-1]]
print("Case 4:")
for y in a[1:3]:
    print(y)
    
# Case 5: seq[::stride] = Case 1 .(every n=stride)
print("Case 5:")
for y in a[::2]:
    print(y)

# Case 6: seq[low::stride] = Case 2 .(every n=stride)
print("Case 6:")
for y in a[1::2]:
    print(y)

# Case 7: seq[:high:stride] = Case 3 .(every n=stride)
print("Case 7:")
for y in a[:2:2]:
    print(y)
    
# Case 8: seq[low:high:stride] = Case 4 .(every n=stride)
print("Case 8:")
for y in a[1:3:2]:
    print(y)
    
# Case 9: seq[::-1] = Case 1.Reverse()
print("Case 9:")
for y in a[::-1]:
    print(y)
    
# Case 10: seq[high::-1] = Case 2.Reverse()
print("Case 10:")
for y in a[1::-1]:
    print(y)
    
# Case 11: seq[:low:-1] = Case 3.Reverse()
print("Case 11:")
for y in a[:2:-1]:
    print(y)
    
# Case 12: seq[high:low:-1] = Case 4.Reverse()
print("Case 12:")
for y in a[3:1:-1]:
    print(y)