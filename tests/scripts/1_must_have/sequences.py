x = 123

L = [ 1, 2.5, (3,4,5), [], {}, "text", None, x, True ]
print(L)
print()

T = ( 1, 2.5, (3,4,5), [], {}, "text", None, x, True )
print(T)
print()

D = { 'a':1, 1:2.5, 'tuple':(3,4,5), (0,1):[], (1,0):{}, "text":"text", 2.5:None, 0:x }
print(D)
print()

S = { 1, 2.5, (3,4,5), "text", None, x, True}
print('_TEST_UNORDERED_ON')
print(S)
print('_TEST_UNORDERED_OFF')
print()
