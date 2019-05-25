import csv
import sys

objdump = open(sys.argv[1] + sys.argv[2], "r")
hex = []
asm = []

for line in objdump:
	# Skip blank lines
	if len(line) > 1:
		# If the line ends with a colon, it marks the beginning of a section
		if line[-2] == ':':
			# If the previous function has nothing in it, delete it
			if len(hex) > 0 and len(hex[-1]) == 0:
				hex.pop()
				asm.pop()

			hex.append([])
			asm.append([])
		else:
			# Make sure we're inside a function
			if len(hex) > 0:
				# Extract the assembly from the line
				hex[-1].append(line.split("\t")[1][:-1])
				asm[-1].append(" ".join(line.split("\t")[2:])[:-1])

objdump.close()

hexFile = open(sys.argv[1] + "binary_good.csv", "w")
hexWriter = csv.writer(hexFile)

for function in hex:
	hexWriter.writerow(function)

hexFile.close()

asmFile = open(sys.argv[1] + "asm_good.csv", "w")
asmWriter = csv.writer(asmFile)

for function in asm:
	asmWriter.writerow(function)

asmFile.close()
