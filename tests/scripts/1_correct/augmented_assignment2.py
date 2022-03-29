def fun(arg):
    print(arg)
    return arg

L = [ 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 ]
print(L)

L[fun(0)] += 20
print(L)

L[fun(1)] -= 20
print(L)

print("_TEST_UNORDERED_ON")
L[fun(2)] /= 2
print(L)

L[fun(3)] *= 2
print(L)

L[fun(4)] %= 4
print(L)

L[fun(5)] <<= 1
print(L)

L[fun(6)] >>= 1
print(L)

L[fun(7)] |= 15
print(L)

L[fun(8)] &= 15
print(L)

L[fun(9)] ^= 15
print(L)
print("_TEST_UNORDERED_OFF")