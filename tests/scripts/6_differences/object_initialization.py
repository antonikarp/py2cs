class A:
    def __init__(self):
        print("object of class A was created")

class B(A):
    def __init__(self):
        print("object of class B was created")

b = B()
