tab_fun = []
for n in range(5):
    def f():
        return n*n
    tab_fun.append(f)

for i in range(5):
    print(tab_fun[i]())
