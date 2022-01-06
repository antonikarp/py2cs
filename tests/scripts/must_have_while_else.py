i=1
while i<=5:
    print(i)
    i = i+1
else:
    print("No break")
print("*****")
i=1
while True:
    print(i)
    i = i+1
    if ( i>5 ):
        break
else:
    print("No break")
print("*****")
while False:
    pass
else:
    print("No break")
