x = z = 0
for i in range(3):
    def outer_fun():
        global x
        x += 1
        y = 0
        for i in range(3):
            def inner_fun():
                nonlocal y
                global z
                y += 1
                z += 1
                print(x,y,z)
            inner_fun()
    outer_fun()    
