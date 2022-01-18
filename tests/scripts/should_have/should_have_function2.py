tab_fun1 = []
for i in range(5):
    def f(n=i):
        return n*n
    tab_fun1.append(f)

for i in range(5):
    print(tab_fun1[i]())
