import os.path
import sys
from termcolor import colored

first_path = sys.argv[1]
second_path = sys.argv[2]
error_msg_path = sys.argv[3]
name = sys.argv[4]

# If an error message exists, the test is passing.
if (os.path.isfile(error_msg_path)):
    with open(error_msg_path, "r") as file:
        for line in file:
            if line.startswith("Not handled") or line.startswith("Error"):
                print(colored(name + " failed (gracefully).", "yellow"))
                exit()



if (not os.path.isfile(first_path) or not os.path.isfile(second_path)):
    print(colored(name + " failed.", "red"))
    exit()

if (os.path.getsize(first_path) == 0 or os.path.getsize(second_path) == 0):
    print(colored(name + " failed.", "red"))
    exit()
    
lines1 = []
lines2 = []

with open(first_path, "r") as first_file:
    for line in first_file:
        lines1.append(line)

with open(second_path, "r") as second_file:
    for line in second_file:
        lines2.append(line)

# Different number of lines
if len(lines1) != len(lines2):
    print(colored(name + " failed (gracefully).", "yellow"))
    exit()
    
for i in range(len(lines1)):
    if lines1[i] != lines2[i]:
        print(colored(name + " failed (gracefully).", "yellow"))
        exit()
        
# If we are here then we have passed the test (the results are the same).    
print(colored(name + " passed.", "green"))



