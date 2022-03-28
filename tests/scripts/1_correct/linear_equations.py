print('System of linear equations')

a = [ [2.0,3.0], [4.0,5.0] ]
b = [ 12.0, 22.0 ]
x = [ 0, 0 ]
wx = [ 0, 0 ]

print("_TEST_UNORDERED_ON")

print(a[0][0],"x +",a[0][1],"y =",b[0])
print(a[1][0],"x +",a[1][1],"y =",b[1])

w = a[0][0]*a[1][1]-a[0][1]*a[1][0]
wx[0] = b[0]*a[1][1]-a[0][1]*b[1]
wx[1] = a[0][0]*b[1]-b[0]*a[1][0]

if w==0.0:
    print("Syatem is inconsistent or indeterminated")
else:
    x[0] = wx[0]/w
    x[1] = wx[1]/w
    print("x =",x[0])
    print("y =",x[1])

print("_TEST_UNORDERED_OFF")

