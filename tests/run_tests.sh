#!/bin/bash

names=(test1 test2)

# Clean the test directories first. We need only the Python scripts/
rm generated/*
rm generated_output/*
rm scripts_output/*

for name in "${names[@]}"
do
	# Run the test script and get its output.
	python3 scripts/"$name".py > scripts_output/"$name".txt

	# Perform translation.
	cd ..
	dotnet test
	cd tests

	# Compile the obtained .cs file.
	mcs generated/"$name".cs

	# Run the compiled program and save its output.
	mono generated/"$name".exe > generated_output/"$name".txt

	# Compare the two outputs. If they match, then the test passed.
	python3 compare_results.py ./scripts_output/"$name".txt ./scripts_output/"$name".txt "$name"

done