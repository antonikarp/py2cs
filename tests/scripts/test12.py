# Default parameters. Named parameters.

def sub(a=0, b=1):
	return a-b

print(sub(1, 3))
print(sub())
print(sub(b=2, a=4))
print(sub(b=2))
