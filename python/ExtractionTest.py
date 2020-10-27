import os, sys, gc, ubinascii, umachine

currentFile = __file__

with open(currentFile) as file:
    outputFile = None
    for line in file:
        if line[:8] == "# BINARY":
            outputFile = open("/tmpmodule.mpy", 'wb')
        elif outputFile != None:
            bts = ubinascii.a2b_base64(line[1:-1])
            outputFile.write(bts)
    if outputFile != None:
        outputFile.close()
    os.sync()

import tmpmodule
try:
    tmpmodule.code()
finally:
    del tmpmodule
    del sys.modules["tmpmodule"]
    os.remove("/tmpmodule.mpy")
    gc.collect()
    gc.mem_free()

# File just needs to be named tmpmodule.py, and base64 encoded

# BINARY
#TQUCHyCDRDASAAccLlx0bXBtb2R1bGUucHkfPh9DAIAQCk1TSHViEApNb3RvchASTW90b3JQYWly
#EBZDb2xvclNlbnNvchAcRGlzdGFuY2VTZW5zb3IQBkFwcCoGGxRtaW5kc3Rvcm1zHA0WARwNFgEc
#DRYBHA0WARwNFgEcDRYBWYAQIHdhaXRfZm9yX3NlY29uZHMQFHdhaXRfdW50aWwQClRpbWVyKgMb
#JG1pbmRzdG9ybXMuY29udHJvbBwHFgEcBxYBHAcWAVkyABYIY29kZVFjAAGBOBgUARlgIEkpJwAS
#FRACQTQBwLAUAJIiMjYBWRINgjQBWbAUAJY2AFlRYwAA


