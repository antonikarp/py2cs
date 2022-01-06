#!/bin/bash

# First delete the previous generated files.
bash ./clean_dirs.sh

# Perform translation.
cd ..
dotnet test
cd tests

cat testnames.txt | while read name
do
	# Run the test script and get its output.
	python3 scripts/"$name".py > scripts_output/"$name".txt

	# Run the compiled program and save its output.
	mono generated/"$name".exe > generated_output/"$name".txt

	# Compare the two outputs. If they match, then the test passed.
	python3 compare_results.py ./generated_output/"$name".txt ./scripts_output/"$name".txt "$name"
done
