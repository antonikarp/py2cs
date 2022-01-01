# Handling of exceptions.

a = 0
b = 0
try:
    c = a / b
except ZeroDivisionError:
    print("Division by zero.")
finally:
    print("Finally.")