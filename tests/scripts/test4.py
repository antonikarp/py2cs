# Nested if.

a = 3
b = 4
c = 5
if a > c:
    print("a")
    if a > b:
        print("aa")
    else:
        print("ab")
elif b > c:
    print("b")
else:
	print("c")