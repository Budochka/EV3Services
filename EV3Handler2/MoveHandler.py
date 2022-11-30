import logging

from ev3dev2.motor import (OUTPUT_A, OUTPUT_B, OUTPUT_C, LargeMotor,
                           MediumMotor, MoveTank, SpeedPercent)


def callback_move(ch, method, properties, body):
    tank_drive = MoveTank(OUTPUT_B, OUTPUT_C)
    head = MediumMotor(OUTPUT_A)


    if (method.routing_key.find('.turn') != -1):
        logging.info('Turn message processed')
        degree = int.from_bytes(body[0:3], byteorder ='little')
        torque = int.from_bytes(body[4:7], byteorder ='little', signed=True)

        tank_drive.on_for_degrees(SpeedPercent(-torque), SpeedPercent(torque), degree*5)
        return

    if (method.routing_key.find('.distance') != -1):
        logging.info('Move message processed')
        distance = int.from_bytes(body[0:3], byteorder ='little', signed=True)
        torque = int.from_bytes(body[4:7], byteorder ='little')

        tank_drive.on_for_rotations(SpeedPercent(torque), SpeedPercent(torque), distance/12)
        return

    if (method.routing_key.find('.headturn') != -1):
        logging.info('HeadTurn message processed')
        degree = int.from_bytes(body[0:3], byteorder ='little')
        torque = int.from_bytes(body[4:7], byteorder ='little', signed=True)

        GEARS_SIZE_CORRECTION = 3.4 #coefficient to compensate difference in gears size so small one should move to bigger angle
        head.on_for_degrees(torque, degree * GEARS_SIZE_CORRECTION)
        return
