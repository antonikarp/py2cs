#!/bin/bash

# First delete the previous generated files.
bash ./clean_test_dirs.sh

# Perform translation.
cd ..
dotnet test
cd tests

# Tests which give the same results.
dir_names_1=(1_must_have 2_should_have 3_nice_to_have)

for dir_name in "${dir_names_1[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name 
	do
		# Run the test script and get its output.
		python3 -O scripts/"$dir_name"/"$name".py > scripts_output/"$dir_name"/"$name".txt

		cd ..
		dotnet run tests/scripts/"$dir_name"/"$name".py tests/generated/"$dir_name"
		cd tests
		
		echo
		
		csc generated/"$dir_name"/"$name"*.cs /out:generated/"$dir_name"/"$name".exe
		
		# Run the compiled program and save its output.
		mono generated/"$dir_name"/"$name".exe > generated_output/"$dir_name"/"$name".txt
	done
		
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		# Compare the two outputs. If they match, then the test passed.
		python3 compare_results_identical.py ./generated_output/"$dir_name"/"$name".txt ./scripts_output/"$dir_name"/"$name".txt "${dir_name}/${name}"
	done
done




# Tests which give different results in Python and C#.
dir_names_2=(6_differences)

for dir_name in "${dir_names_2[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		# Run the test script and get its output.
		python3 scripts/"$dir_name"/"$name".py > scripts_output/"$dir_name"/"$name".txt

		# Run the compiled program and save its output.
		mono generated/"$dir_name"/"$name".exe > generated_output/"$dir_name"/"$name".txt
		
		# Compare the two outputs. They are expected to be different
		python3 compare_results_different.py ./generated_output/"$dir_name"/"$name".txt ./scripts_output/"$dir_name"/"$name".txt "${dir_name}/${name}"
	done
done

# Tests which check scripts with language constructs which are not handled.
dir_names_3=(4_not_implemented)
for dir_name in "${dir_names_3[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		# There is no need to run scripts (which might not be correct), because we don't check its results. 

		# Instead of .cs file there is a generated .txt file
		python3 validate_results_not_implemented.py ./generated/"$dir_name"/"$name".txt "${dir_name}/${name}"
	done
done

# Tests of programs which generate an error
# Currently the tool recognizes the following error:
# 1. The execution of the script was unsuccessful. The script wrote error message to stderr
#    This includes at runtime.
# 2. The process running the script exceeded the memory limit
# 3. The execution of the script was successful, but the source code is unable to be parsed
#    By the current grammar. This might happen for some language features that were added after Python 3.6.
# In each of these cases, a text file is generated which contains the description of the error.

dir_names_4=(5_error)

for dir_name in "${dir_names_4[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		# Instead of .cs file there is a generated .txt file
		python3 validate_results_error.py ./generated/"$dir_name"/"$name".txt "${dir_name}/${name}"
	done
done


# Tests checking the import mechanism
dir_names_5=(1 2 3 4)

for dir_name in "${dir_names_5[@]}"
do
	cat scripts/1_must_have/import/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		# Run the test script and get its output.
	
		python3 scripts/1_must_have/import/"$dir_name"/"$name".py > scripts_output/1_must_have/import/"$dir_name"/"$name".txt
		
		cd ..
		dotnet run tests/scripts/1_must_have/import/"$dir_name"/"$name".py tests/generated/1_must_have/import/"$dir_name"
		cd tests
		
		echo
		
		csc generated/1_must_have/import/"$dir_name"/*.cs /out:generated/1_must_have/import/"$dir_name"/"$name".exe

		# Run the compiled program and save its output.
		mono generated/1_must_have/import/"$dir_name"/"$name".exe > generated_output/1_must_have/import/"$dir_name"/"$name".txt
	done	
done

for dir_name in "${dir_names_5[@]}"
do
	cat scripts/1_must_have/import/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		# Compare the two outputs. If they match, then the test passed.
		python3 compare_results_identical.py ./generated_output/1_must_have/import/"$dir_name"/"$name".txt ./scripts_output/1_must_have/import/"$dir_name"/"$name".txt "1_must_have/import/${dir_name}"
	done
done

# Tests that take data to stdin from external files (.in)
dir_names_6=(1_must_have/input)

for dir_name in "${dir_names_6[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		# Run the test script with the provided file for stdin and get its output.
		python3 scripts/"$dir_name"/"$name".py < scripts/"$dir_name"/"$name".in > scripts_output/"$dir_name"/"$name".txt
		
		# Run the compiled program supplying the input file and save its output.
		mono generated/"$dir_name"/"$name".exe < scripts/"$dir_name"/"$name".in > generated_output/"$dir_name"/"$name".txt
		
		# Compare the two outputs. If they match, then the test passed.
		python3 compare_results_identical.py ./generated_output/"$dir_name"/"$name".txt ./scripts_output/"$dir_name"/"$name".txt "${dir_name}/${name}"
		
	done
done
echo ""


