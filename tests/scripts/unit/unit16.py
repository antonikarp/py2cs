# Handling of exceptions.

a = 0
b = 0
try:
    c = a / b
except ZeroDivisionError:
    print("Division by zero.")
finally:
    print("Finally.")

d = [1, 2, 3]
try:
    e = d[4]
except IndexError:
    print("Index out of range")