import os.path
import sys
import filecmp
from termcolor import colored

first_path = sys.argv[1]
second_path = sys.argv[2]
name = sys.argv[3]

# If both files don't exist, the test is failing.
if (not os.path.isfile(first_path) and not os.path.isfile(second_path)):
    print(colored(name + " failed.", "red"))

# If just one file exists, the test is passing.
elif (os.path.isfile(first_path) and not os.path.isfile(second_path)):
    print(colored(name + " passed.", "green"))
elif (not os.path.isfile(first_path) and os.path.isfile(second_path)):
    print(colored(name + " passed.", "green"))
else:
    result = filecmp.cmp(first_path, second_path, shallow=False)
    # The contents of the files are expected to be different.
    if result == False:
        print(colored(name + " passed.", "green"))
    else:
        print(colored(name + " failed.", "red"))