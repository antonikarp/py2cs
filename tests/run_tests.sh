#!/bin/bash

# First delete the previous generated files.
bash ./clean_dirs.sh

# Perform translation.
cd ..
dotnet test
cd tests

# Tests which give the same results.
dir_names_1=(must_have should_have nice_to_have unit)

for dir_name in "${dir_names_1[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name 
	do
		# Run the test script and get its output.
		python3 scripts/"$dir_name"/"$name".py > scripts_output/"$dir_name"/"$name".txt

		# Run the compiled program and save its output.
		mono generated/"$dir_name"/"$name".exe > generated_output/"$dir_name"/"$name".txt

		# Compare the two outputs. If they match, then the test passed.
		python3 compare_results_identical.py ./generated_output/"$dir_name"/"$name".txt ./scripts_output/"$dir_name"/"$name".txt "$name"
		
	done
done

# Tests which give different results in Python and C#.
dir_names_2=(difference)

for dir_name in "${dir_names_2[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		# Run the test script and get its output.
		python3 scripts/"$dir_name"/"$name".py > scripts_output/"$dir_name"/"$name".txt

		# Run the compiled program and save its output.
		mono generated/"$dir_name"/"$name".exe > generated_output/"$dir_name"/"$name".txt
		
		# Compare the two outputs. They are expected to be different
		python3 compare_results_different.py ./generated_output/"$dir_name"/"$name".txt ./scripts_output/"$dir_name"/"$name".txt "$name"
	done
done

# Tests which check scripts with language constructs which are not handled.
dir_names_3=(not_implemented)
for dir_name in "${dir_names_3[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		# There is no need to run scripts (which might not be correct), because we don't check its results. 

		# Instead of .cs file there is a generated .txt file
		python3 compare_results_not_implemented.py ./generated/"$dir_name"/"$name".txt "$name"
	done
done

# Tests of programs which generate error
# The results of translation of an incorrect program could be the following:
# a) error at parsing -> this is handled and a text file with message is created in 'generated'
# b) code can't be compiled
# c) code can be compiled, run and an output is produced
# d) code can be compiled, but has a runtime error
#
# Due to these possibilities, it is hard to determine whether an 'error' test succeeded or not.


echo ""


