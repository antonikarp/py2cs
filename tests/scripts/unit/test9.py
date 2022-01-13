# Sets and dicts.

# Due to unspecified ordered of iteration over keys, the value which is printed
# is constant.
new_set = {"a", "b", "c"}
if "a" in new_set:
	print("Yes")
else:
	print("No")

for x in new_set:
	print("set")

new_dict = {"d": 1, "e": 2, "f": 3}
if "h" in new_dict:
	print("Yes")
else:
	print("No")

for y in new_dict:
	print("dict")