#SLOT2
#COMPILE

import os

print(os.listdir('/projects'))
log = open("/projects/23262.py", "r")

for line in log:
    print(line)
