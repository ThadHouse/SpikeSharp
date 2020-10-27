import ubinascii

f = open("/projects/standalone.mpy", 'rb')
hexdata = ubinascii.hexlify(f.read())
print("Handling")
print(hexdata[:40])
f.close()

f = open("/tmpmodule.mpy", 'rb')
hexdata = ubinascii.hexlify(f.read())
print("Handling")
print(hexdata[:40])
f.close()
