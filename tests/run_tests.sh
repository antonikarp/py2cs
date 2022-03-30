#!/bin/bash

# First delete the previous generated files.
bash ./clean_test_dirs.sh

# Generate the names of the files used for testing.
cd ..
dotnet test
cd tests

# 1a. -------------- Tests in which the results are the same. Import mechanism.
dir_names_1=(1 2 3 4 5)

for dir_name in "${dir_names_1[@]}"
do
	cat scripts/1_correct/import/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		# Run the test script and get its output.
	
		python3 scripts/1_correct/import/"$dir_name"/"$name".py > scripts_output/1_correct/import/"$dir_name"/"$name".txt
		
		cd ..
		dotnet run tests/scripts/1_correct/import/"$dir_name"/"$name".py tests/generated/1_correct/import/"$dir_name" --nodelete
		cd tests
		
		echo
		
		csc generated/1_correct/import/"$dir_name"/*.cs /out:generated/1_correct/import/"$dir_name"/"$name".exe

		# Run the compiled program and save its output.
		mono generated/1_correct/import/"$dir_name"/"$name".exe > generated_output/1_correct/import/"$dir_name"/"$name".txt
		
		echo
	done	
done

# 1b. -------------- Tests in which the result are the same. Take data to stdin from external files (.in)
dir_names_2=(1_correct/input)

for dir_name in "${dir_names_2[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do	
		# Run the test script with the provided file for stdin and get its output.
		python3 scripts/"$dir_name"/"$name".py < scripts/"$dir_name"/"$name".in > scripts_output/"$dir_name"/"$name".txt
		
		cd ..
		dotnet run tests/scripts/"$dir_name"/"$name".py tests/generated/"$dir_name" --nodelete
		cd tests
		
		echo
		
		csc generated/"$dir_name"/"$name"*.cs /out:generated/"$dir_name"/"$name".exe
		
		# Run the compiled program supplying the input file and save its output.
		mono generated/"$dir_name"/"$name".exe < scripts/"$dir_name"/"$name".in > generated_output/"$dir_name"/"$name".txt
		
		echo
	done
done

# 1c. -------------- Tests in which the script and the translated program give the same results.
dir_names_3=(1_correct)

for dir_name in "${dir_names_3[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name 
	do
		# Run the test script and get its output.
		python3 -O scripts/"$dir_name"/"$name".py > scripts_output/"$dir_name"/"$name".txt

		cd ..
		dotnet run tests/scripts/"$dir_name"/"$name".py tests/generated/"$dir_name" --nodelete
		cd tests
		
		echo
		
		csc generated/"$dir_name"/"$name"*.cs /out:generated/"$dir_name"/"$name".exe /langversion:8.0
		
		# Run the compiled program and save its output.
		mono generated/"$dir_name"/"$name".exe > generated_output/"$dir_name"/"$name".txt
		
		echo
	done	
done

# 2. -------------- Tests which check scripts with language constructs which are not handled.
dir_names_4=(2_not_implemented)
for dir_name in "${dir_names_4[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		# There is no need to run scripts (which might not be correct), because we don't check its results. 
		cd ..
		dotnet run tests/scripts/"$dir_name"/"$name".py tests/generated/"$dir_name" --nodelete
		cd tests
		
		echo
	done
done

# 3. -------------- Tests of programs which generate an error. The result of translation is a message with an error.

dir_names_5=(3_incorrect_scripts)

for dir_name in "${dir_names_5[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		cd ..
		dotnet run tests/scripts/"$dir_name"/"$name".py tests/generated/"$dir_name" --nodelete
		cd tests
		
		echo
	done
done

# 4. -------------- Tests which give different results in Python and C#.
dir_names_6=(4_differences)

for dir_name in "${dir_names_6[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		# Run the test script and get its output.
		python3 scripts/"$dir_name"/"$name".py 1> scripts_output/"$dir_name"/"$name".txt 2> /dev/null
		
		cd ..
		dotnet run tests/scripts/"$dir_name"/"$name".py tests/generated/"$dir_name" --nodelete
		cd tests
		
		echo
		
		csc generated/"$dir_name"/"$name"*.cs /out:generated/"$dir_name"/"$name".exe

		# Run the compiled program and save its output.
		mono generated/"$dir_name"/"$name".exe > generated_output/"$dir_name"/"$name".txt
		
		echo
	done
done

echo

echo "----------RESULTS----------"

for dir_name in "${dir_names_1[@]}"
do
	cat scripts/1_correct/import/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		# Compare the two outputs. If they match, then the test passed.
		python3 compare_results_identical.py ./generated_output/1_correct/import/"$dir_name"/"$name".txt ./scripts_output/1_correct/import/"$dir_name"/"$name".txt "1_correct/import/${dir_name}"
	done
done

echo

for dir_name in "${dir_names_2[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		# Compare the two outputs. If they match, then the test passed.
		python3 compare_results_identical.py ./generated_output/"$dir_name"/"$name".txt ./scripts_output/"$dir_name"/"$name".txt "${dir_name}/${name}"
	done
	echo
done


for dir_name in "${dir_names_3[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		# Compare the two outputs. If they match, then the test passed.
		python3 compare_results_identical.py ./generated_output/"$dir_name"/"$name".txt ./scripts_output/"$dir_name"/"$name".txt "${dir_name}/${name}"
	done
	echo
done

for dir_name in "${dir_names_4[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		# Instead of .cs file there is a generated .txt file
		python3 validate_results_not_implemented.py ./generated/"$dir_name"/"$name".txt "${dir_name}/${name}"
	done
	echo
done

for dir_name in "${dir_names_5[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do
		# Instead of .cs file there is a generated .txt file
		python3 validate_results_error.py ./generated/"$dir_name"/"$name".txt "${dir_name}/${name}"
	done
	echo
done

for dir_name in "${dir_names_6[@]}"
do
	cat scripts/"$dir_name"/testnames.txt 2> /dev/null | while read name
	do	
		# Compare the two outputs. They are expected to be different
		python3 compare_results_different.py ./generated_output/"$dir_name"/"$name".txt ./scripts_output/"$dir_name"/"$name".txt "${dir_name}/${name}"
	done
	echo
done

echo


