from mindstorms import MSHub, Motor, MotorPair, ColorSensor, DistanceSensor, App
from mindstorms.control import wait_for_seconds, wait_until, Timer

def code():
    rightArm = Motor('A')

    rightArm.start(50)
    wait_for_seconds(2)
    rightArm.stop()

# import sys

# def code():
#     sys_mpy = sys.implementation.mpy
#     arch = [None, 'x86', 'x64',
#         'armv6', 'armv6m', 'armv7m', 'armv7em', 'armv7emsp', 'armv7emdp',
#         'xtensa', 'xtensawin'][sys_mpy >> 10]
#     print('mpy version:', sys_mpy & 0xff)
#     print('mpy flags:', end='')
#     print(sys_mpy)
#     if arch:
#         print(' -march=' + arch, end='')
#     if sys_mpy & 0x100:
#         print(' -mcache-lookup-bc', end='')
#     if not sys_mpy & 0x200:
#         print(' -mno-unicode', end='')
#     print()
