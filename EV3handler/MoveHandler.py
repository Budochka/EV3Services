import logging
from ev3dev2.motor import LargeMotor, OUTPUT_A, OUTPUT_B, OUTPUT_C, SpeedPercent, MoveTank

def callback_move(ch, method, properties, body):
    tank_drive = MoveTank(OUTPUT_B, OUTPUT_C)


    if (method.routing_key.find('.turn') != -1):
        logging.info('Turn message processed')
        CCW = 1
        if (body[0] > 127):
            CCW = -1
        tank_drive.on_for_seconds(SpeedPercent(-50*CCW ), SpeedPercent(50*CCW), 1)
        return

    if (method.routing_key.find('.distance') != -1):
        logging.info('Move message processed')
        move_direction = 1
        if (body[0] > 127):
            move_direction = -1
        tank_drive.on_for_seconds(SpeedPercent(60 * move_direction), SpeedPercent(60 * move_direction), 1)
        return
