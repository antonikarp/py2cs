import sys
from termcolor import colored

path = sys.argv[1]
name = sys.argv[2]

with open(path, "r") as file:
    for line in file:
        if line.startswith("Error") or line.startswith("Syntax error"):
            print(colored(name + " passed.", "green"))
            exit()

print(colored(name + " failed.", "red"))