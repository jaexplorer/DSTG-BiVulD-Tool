import csv
import sys

objdump = open(sys.argv[1] + sys.argv[2], "r")
functions = []

for line in objdump:
	# Skip blank lines
	if len(line) > 1:
		# If the line ends with a colon, it marks the beginning of a section
		if line[-2] == ':':
			# If the previous function has nothing in it, delete it
			if len(functions) > 0 and len(functions[-1]) == 0:
				functions.pop()

			functions.append([])
		else:
			# Make sure we're inside a function
			if len(functions) > 0:
				# Extract the assembly from the line
				functions[-1].append(line.split("\t")[1][:-1])

objdump.close()

output = open(sys.argv[1] + "binary_good.csv", "w")
csvWriter = csv.writer(output)

for function in functions:
	csvWriter.writerow(function)

output.close()
