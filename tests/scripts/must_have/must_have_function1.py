def pow(y):
    def f(x):
        return x**y
    return f

sqr = pow(2)
sqrt = pow(0.5)

print(sqr(2.0))
print(sqrt(9.0))
print(pow(3)(2.0))
