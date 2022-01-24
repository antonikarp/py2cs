import sys
from termcolor import colored

def is_number(s):
    try:
        float(s)
        return True
    except ValueError:
        return False

first_path = sys.argv[1]
second_path = sys.argv[2]
name = sys.argv[3]

eps = 0.0001

lines1 = []
lines2 = []

# unordered:
# When printing sets the order doesn't matter, but the test should still be passing on
# such different lines. If unordered == True then both lines are printed and even if
# they are different, the test won't fail.

unordered = False

with open(first_path, "r") as first_file:
    for line in first_file:
        lines1.append(line)

with open(second_path, "r") as second_file:
    for line in second_file:
        lines2.append(line)

# Different number of lines
if len(lines1) != len(lines2):
    print(colored(name + " failed.", "red"))
    exit()
    
for i in range(len(lines1)):
    if (is_number(lines1[i])) and (is_number(lines2[i])):
        # The difference is greater than eps
        if (abs(float(lines1[i]) - float(lines2[i])) > eps):
            print(colored(name + " failed.", "red"))
            exit()
    else:
        # Remove the \n character
        lines1[i] = lines1[i][:-1]
        lines2[i] = lines2[i][:-1]
        if lines1[i] == lines2[i] and lines1[i] == "_TEST_UNORDERED_ON":
            unordered = True
        elif lines1[i] == lines2[i] and lines1[i] == "_TEST_UNORDERED_OFF":
            print("********************")
            unordered = False
        elif unordered and len(lines1[i]) > 0 and len(lines2[i]) > 0:
            print("********************")
            print("line1 = ", lines1[i])
            print("line2 = ", lines2[i])
            print("Are they equivalent?")
            
        # Lines as strings (not numbers) are different.
        elif lines1[i] != lines2[i] and (not unordered):
            print(colored(name + " failed.", "red"))
            exit()

# If we are here then we have passed the test.    
print(colored(name + " passed.", "green"))