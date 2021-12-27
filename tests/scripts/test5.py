# List. For loop. Break. Continue.
a = [3, 4, 5]
b = [1, 2]
for x in a:
	for y in b:
		print(x + y)

c = [2, 3, 4, 5]
for x in c:
	print(x)
	if x == 2:
		continue
	if x == 3:
		break