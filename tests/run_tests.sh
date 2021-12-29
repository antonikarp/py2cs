#!/bin/bash

names=(test1 test2 test3 test4 test5 test6 test7 test8 test9 test10)

# First delete the previous generated files.
bash ./clean_dirs.sh

# Perform translation.
cd ..
dotnet test
cd tests

for name in "${names[@]}"
do
	# Run the test script and get its output.
	python3 scripts/"$name".py > scripts_output/"$name".txt

	# Compile the obtained .cs file.
	mcs generated/"$name".cs

	# Run the compiled program and save its output.
	mono generated/"$name".exe > generated_output/"$name".txt

	# Compare the two outputs. If they match, then the test passed.
	python3 compare_results.py ./generated_output/"$name".txt ./scripts_output/"$name".txt "$name"

done