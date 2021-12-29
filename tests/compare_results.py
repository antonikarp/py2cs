import sys
import filecmp
from termcolor import colored

first_path = sys.argv[1]
second_path = sys.argv[2]
name = sys.argv[3]

result = filecmp.cmp(first_path, second_path, shallow=False)
if result==True:
	print(colored(name + " passed.", "green"))
else:
	print(colored(name + " failed.", "red"))