import sys
from termcolor import colored

path = sys.argv[1]
name = sys.argv[2]

with open(path, "r") as file:
    for line in file:
        if line.startswith("Not handled") or line.startswith("Error"):
            print(colored(name + " passed.", "green"))
            exit()

print(colored(name + " failed.", "red"))
            