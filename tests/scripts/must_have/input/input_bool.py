def str_to_bool(s):
    tf = {"True":True, "False":False}
    try:
        return tf[s]
    except Exception:
        raise Exception('incorrect bool value')

b1 = str_to_bool(input('input first bool value:  '))
print(b1)
b2 = str_to_bool(input('input second bool value:  '))
print(b2)
print('first or second:   ', b1 or b2)
print('second and first:  ', b2 and b1)
