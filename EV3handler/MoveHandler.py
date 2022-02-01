import logging
from pickle import TRUE
from ev3dev2.motor import LargeMotor, OUTPUT_A, OUTPUT_B, OUTPUT_C, SpeedPercent, MoveTank

def callback_move(ch, method, properties, body):
    tank_drive = MoveTank(OUTPUT_B, OUTPUT_C)


    if (method.routing_key.find('.turn') != -1):
        logging.info('Turn message processed')
        degree = int.from_bytes(body[0:3], byteorder ='little')
        torque = int.from_bytes(body[4:7], byteorder ='little')

        tank_drive.on_for_seconds(SpeedPercent(-torque), SpeedPercent(torque), 1)
        return

    if (method.routing_key.find('.distance') != -1):
        logging.info('Move message processed')
        distance = int.from_bytes(body[0:3], byteorder ='little', signed=TRUE)
        torque = int.from_bytes(body[4:7], byteorder ='little')


        tank_drive.on_for_seconds(SpeedPercent(torque), SpeedPercent(torque), distance/60)
        return
