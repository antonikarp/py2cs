class Person:
    def __init__(self, firstName, lastName):
        self.firstName = firstName
        self.lastName = lastName
    def get(self):
        print(self.firstName + self.lastName)
class Student(Person):
    def __init__(self, firstName, lastName, age):
        Person.__init__(self, firstName, lastName)
        self.age = age
x = Student("John", "Doe", 20)
x.get()