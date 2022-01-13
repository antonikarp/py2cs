#!/bin/bash

# First delete the previous generated files.
bash ./clean_dirs.sh

# Perform translation.
cd ..
dotnet test
cd tests

dir_names=(difference error must_have nice_to_have not_implemented should_have unit)

for dir_name in "${dir_names[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name 
	do
		# Run the test script and get its output.
		python3 scripts/"$dir_name"/"$name".py > scripts_output/"$dir_name"/"$name".txt

		# Run the compiled program and save its output.
		mono generated/"$dir_name"/"$name".exe > generated_output/"$dir_name"/"$name".txt

		# Compare the two outputs. If they match, then the test passed.
		python3 compare_results.py ./generated_output/"$dir_name"/"$name".txt ./scripts_output/"$dir_name"/"$name".txt "$name"
		
	done
done
echo ""


