#!/bin/bash

echo "Cleaning directories..."

# Tests with inputs:
rm scripts/must_have/input/testnames.txt 2> /dev/null
rm scripts_output/must_have/input/* 2> /dev/null
rm generated/must_have/input/* 2> /dev/null
rm generated_output/must_have/input/* 2> /dev/null

# Tests with imports:
rm scripts/must_have/import/1/testnames.txt 2> /dev/null
rm scripts/must_have/import/2/testnames.txt 2> /dev/null
rm scripts/must_have/import/3/testnames.txt 2> /dev/null
rm scripts/must_have/import/4/testnames.txt 2> /dev/null

rm scripts_output/must_have/import/1/* 2> /dev/null
rm scripts_output/must_have/import/2/* 2> /dev/null
rm scripts_output/must_have/import/3/* 2> /dev/null
rm scripts_output/must_have/import/4/* 2> /dev/null

rm generated/must_have/import/1/* 2> /dev/null
rm generated/must_have/import/2/* 2> /dev/null
rm generated/must_have/import/3/* 2> /dev/null
rm generated/must_have/import/4/* 2> /dev/null

rm generated_output/must_have/import/1/* 2> /dev/null
rm generated_output/must_have/import/2/* 2> /dev/null
rm generated_output/must_have/import/3/* 2> /dev/null
rm generated_output/must_have/import/4/* 2> /dev/null

# Other:

rm scripts/difference/testnames.txt 2> /dev/null
rm scripts/error/testnames.txt 2> /dev/null
rm scripts/must_have/testnames.txt 2> /dev/null
rm scripts/should_have/testnames.txt 2> /dev/null
rm scripts/nice_to_have/testnames.txt 2> /dev/null
rm scripts/not_implemented/testnames.txt 2> /dev/null
rm scripts/should_have/testnames.txt 2> /dev/null
rm scripts/unit/testnames.txt 2> /dev/null

rm scripts_output/difference/* 2> /dev/null
rm scripts_output/error/* 2> /dev/null
rm scripts_output/must_have/* 2> /dev/null
rm scripts_output/should_have/* 2> /dev/null
rm scripts_output/nice_to_have/* 2> /dev/null
rm scripts_output/not_implemented/* 2> /dev/null
rm scripts_output/should_have/* 2> /dev/null
rm scripts_output/unit/* 2> /dev/null

rm generated/difference/* 2> /dev/null
rm generated/error/* 2> /dev/null
rm generated/must_have/* 2> /dev/null
rm generated/should_have/* 2> /dev/null
rm generated/nice_to_have/* 2> /dev/null
rm generated/not_implemented/* 2> /dev/null
rm generated/should_have/* 2> /dev/null
rm generated/unit/* 2> /dev/null

rm generated_output/difference/* 2> /dev/null
rm generated_output/error/* 2> /dev/null
rm generated_output/must_have/* 2> /dev/null
rm generated_output/should_have/* 2> /dev/null
rm generated_output/nice_to_have/* 2> /dev/null
rm generated_output/not_implemented/* 2> /dev/null
rm generated_output/should_have/* 2> /dev/null
rm generated_output/unit/* 2> /dev/null

