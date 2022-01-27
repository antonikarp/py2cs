#!/bin/bash

echo "Cleaning directories..."

# Tests with inputs:
rm scripts/1_must_have/input/testnames.txt 2> /dev/null
rm scripts_output/1_must_have/input/* 2> /dev/null
rm generated/1_must_have/input/* 2> /dev/null
rm generated_output/1_must_have/input/* 2> /dev/null

# Tests with imports:
rm scripts/1_must_have/import/1/testnames.txt 2> /dev/null
rm scripts/1_must_have/import/2/testnames.txt 2> /dev/null
rm scripts/1_must_have/import/3/testnames.txt 2> /dev/null
rm scripts/1_must_have/import/4/testnames.txt 2> /dev/null

rm scripts_output/1_must_have/import/1/* 2> /dev/null
rm scripts_output/1_must_have/import/2/* 2> /dev/null
rm scripts_output/1_must_have/import/3/* 2> /dev/null
rm scripts_output/1_must_have/import/4/* 2> /dev/null

rm generated/1_must_have/import/1/* 2> /dev/null
rm generated/1_must_have/import/2/* 2> /dev/null
rm generated/1_must_have/import/3/* 2> /dev/null
rm generated/1_must_have/import/4/* 2> /dev/null

rm generated_output/1_must_have/import/1/* 2> /dev/null
rm generated_output/1_must_have/import/2/* 2> /dev/null
rm generated_output/1_must_have/import/3/* 2> /dev/null
rm generated_output/1_must_have/import/4/* 2> /dev/null

# Other:

rm scripts/6_differences/testnames.txt 2> /dev/null
rm scripts/5_error/testnames.txt 2> /dev/null
rm scripts/1_must_have/testnames.txt 2> /dev/null
rm scripts/2_should_have/testnames.txt 2> /dev/null
rm scripts/3_nice_to_have/testnames.txt 2> /dev/null
rm scripts/4_not_implemented/testnames.txt 2> /dev/null

rm scripts_output/6_differences/* 2> /dev/null
rm scripts_output/5_error/* 2> /dev/null
rm scripts_output/1_must_have/* 2> /dev/null
rm scripts_output/2_should_have/* 2> /dev/null
rm scripts_output/3_nice_to_have/* 2> /dev/null
rm scripts_output/4_not_implemented/* 2> /dev/null

rm generated/6_differences/* 2> /dev/null
rm generated/5_error/* 2> /dev/null
rm generated/1_must_have/* 2> /dev/null
rm generated/2_should_have/* 2> /dev/null
rm generated/3_nice_to_have/* 2> /dev/null
rm generated/4_not_implemented/* 2> /dev/null

rm generated_output/6_differences/* 2> /dev/null
rm generated_output/5_error/* 2> /dev/null
rm generated_output/1_must_have/* 2> /dev/null
rm generated_output/2_should_have/* 2> /dev/null
rm generated_output/3_nice_to_have/* 2> /dev/null
rm generated_output/4_not_implemented/* 2> /dev/null

