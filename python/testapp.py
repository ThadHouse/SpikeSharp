from mindstorms import MSHub, Motor, MotorPair, ColorSensor, DistanceSensor, App
from mindstorms.control import wait_for_seconds, wait_until, Timer
from mindstorms.operator import equal_to
#from mindstorms.operator import greater_than, greater_than_or_equal_to, less_than, less_than_or_equal_to, equal_to, not_equal_to
import math
import math

ap = App()

rightArm = Motor('F')
leftArm = Motor('B')

def calibrate_charlie():
    rightArm.set_stall_detection(True)
    leftArm.set_stall_detection(True)

    leftArm.start(20)
    rightArm.start(-20)

    print('Starting')

    wait_for_seconds(1)

    print('Run To Position')

    rightArm.run_to_position(0, 'clockwise', 50)
    leftArm.run_to_position(0, 'counterclockwise', 50)

    leftArm.set_degrees_counted(0)
    rightArm.set_degrees_counted(0)

def open_center():
    rightArm.run_to_position(220, 'clockwise', 50)

def center_right_arm():
    if (rightArm.get_degrees_counted() > 0):
        rightArm.run_to_position(0, 'counterclockwise')
    else:
        rightArm.run_to_position(0, 'clockwise')

def tip_hat():
    rightArm.run_to_position(220, 'counterclockwise')

calibrate_charlie()

open_center()

wait_for_seconds(1)

center_right_arm()

wait_for_seconds(1)

tip_hat()

wait_for_seconds(1)

center_right_arm()
