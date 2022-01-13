def fun(arg):
    print(arg)
    return arg

x = fun(1) if fun(2) else fun(3)
print(x)
x = fun(1) if fun(0) else fun(3)
print(x)
print()

x = ( fun(1) if fun(2) else fun(3) ) if ( fun(4) if fun(5) else fun(6) ) else ( fun(7) if fun(8) else fun(9) )
print(x)
print()

x = fun(1) if fun(2) else fun(3) if ( fun(4) if fun(5) else fun(6) ) else ( fun(7) if fun(8) else fun(9) )
print(x)
print()

x = ( fun(1) if fun(2) else fun(3) ) if ( fun(4) if fun(5) else fun(6) ) else fun(7) if fun(8) else fun(9) 
print(x)
print()

x = ( fun(1) if fun(2) else fun(3) ) if fun(0) else fun(7) if fun(8) else fun(9) 
print(x)
print()
