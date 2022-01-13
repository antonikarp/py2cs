def fun(arg):
    print(arg)
    return arg

print('<')
fun(1)<fun(2)<fun(3)<fun(4)<fun(5)<fun(3)<fun(4)
print('<=')
fun(1)<=fun(2)<=fun(3)<=fun(4)<=fun(5)<=fun(3)<=fun(4)
print('>')
fun(5)>fun(4)>fun(3)>fun(2)>fun(1)>fun(3)>fun(2)
print('>=')
fun(5)>=fun(4)>=fun(3)>=fun(2)>=fun(1)>=fun(3)>=fun(2)
print('!=')
fun(1)!=fun(2)!=fun(3)!=fun(2)!=fun(1)!=fun(1)!=fun(3)
print('==')
fun(1)==fun(1)==fun(1)==fun(1)==fun(1)==fun(3)==fun(3)
print('mix')
fun(1)<fun(3)>fun(2)<=fun(4)>=fun(3)==fun(3)!=fun(5)<fun(1)>fun(3)
