#!/bin/bash

echo "Cleaning directories..."

# Tests with inputs:
rm scripts/1_correct/input/testnames.txt 2> /dev/null
rm scripts_output/1_correct/input/* 2> /dev/null
rm generated/1_correct/input/* 2> /dev/null
rm generated_output/1_correct/input/* 2> /dev/null

# Tests with imports:
rm scripts/1_correct/import/1/testnames.txt 2> /dev/null
rm scripts/1_correct/import/2/testnames.txt 2> /dev/null
rm scripts/1_correct/import/3/testnames.txt 2> /dev/null
rm scripts/1_correct/import/4/testnames.txt 2> /dev/null
rm scripts/1_correct/import/5/testnames.txt 2> /dev/null

rm scripts_output/1_correct/import/1/* 2> /dev/null
rm scripts_output/1_correct/import/2/* 2> /dev/null
rm scripts_output/1_correct/import/3/* 2> /dev/null
rm scripts_output/1_correct/import/4/* 2> /dev/null
rm scripts_output/1_correct/import/5/* 2> /dev/null

rm generated/1_correct/import/1/* 2> /dev/null
rm generated/1_correct/import/2/* 2> /dev/null
rm generated/1_correct/import/3/* 2> /dev/null
rm generated/1_correct/import/4/* 2> /dev/null
rm generated/1_correct/import/5/* 2> /dev/null

rm generated_output/1_correct/import/1/* 2> /dev/null
rm generated_output/1_correct/import/2/* 2> /dev/null
rm generated_output/1_correct/import/3/* 2> /dev/null
rm generated_output/1_correct/import/4/* 2> /dev/null
rm generated_output/1_correct/import/5/* 2> /dev/null

# Other:

rm scripts/1_correct/testnames.txt 2> /dev/null
rm scripts/2_not_implemented/testnames.txt 2> /dev/null
rm scripts/3_incorrect_scripts/testnames.txt 2> /dev/null
rm scripts/4_differences/testnames.txt 2> /dev/null

rm scripts_output/1_correct/* 2> /dev/null
rm scripts_output/2_not_implemented/* 2> /dev/null
rm scripts_output/3_incorrect_scripts/* 2> /dev/null
rm scripts_output/4_differences/* 2> /dev/null

rm generated/1_correct/* 2> /dev/null
rm generated/2_not_implemented/* 2> /dev/null
rm generated/3_incorrect_scripts/* 2> /dev/null
rm generated/4_differences/* 2> /dev/null

rm generated_output/1_correct/* 2> /dev/null
rm generated_output/2_not_implemented/* 2> /dev/null
rm generated_output/3_incorrect_scripts/* 2> /dev/null
rm generated_output/4_differences/* 2> /dev/null

