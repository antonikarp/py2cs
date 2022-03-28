for i in range(5):
    print(i)
    i = i+1
else:
    print("No break")
print("*****")
for i in range(10):
    print(i)
    i = i+1
    if ( i>5 ):
        break
else:
    print("No break")
print("*****")
for i in []:
    pass
else:
    print("No break")
