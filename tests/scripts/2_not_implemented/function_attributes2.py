def f():
    f.x += 1
    print(f.x)
f.x = 1
f()
f()