# Nested classes.
class A:
    class B:
        def __init__(self, n):
            self.n = n
        def act(self):
            return self.n + 1
    def __init__(self, m):
        self.m = m
        self.b = self.B(self.m)
    def act(self):
        return self.b.act() + 2
a = A(5)
print(a.act())
