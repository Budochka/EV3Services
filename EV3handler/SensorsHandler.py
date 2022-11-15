import logging
import time

from struct import *
from ev3dev2.sensor.lego import ColorSensor, TouchSensor, UltrasonicSensor


def sensors_reporter(channel, period):
    us = UltrasonicSensor()
    # Put the US sensor into distance mode.
    us.mode = 'US-DIST-CM'

    cl = ColorSensor()
    # Put the color sensor into RGB mode.
    cl.mode='RGB-RAW'

    ts = TouchSensor()
    print("All sensors started")

    finished = False

    while not finished:
        distance = us.value() / 10 #convert to cm
        channel.basic_publish(exchange = 'EV3', routing_key = "sensors.distance", body=pack('f', distance))
        logging.info('Distance sent ' + str(distance))
        print('Distance sent ' + str(distance))

        red = cl.value(0)
        green = cl.value(1)
        blue = cl.value(2)
        channel.basic_publish(exchange = 'EV3', routing_key = "sensors.color", body=bytearray(cl.rgb))
        logging.info('Colors are red=' + str(red) + ', green=' + str(green) + ', blue=' + str(blue))
        print('Colors are red=' + str(red) + ', green=' + str(green) + ', blue=' + str(blue))

        if ts.is_pressed:
            channel.basic_publish(exchange = 'EV3', routing_key = "sensors.touch", body=pack('b', 1))
            logging.info('Touch sensor pushed')
            print('Touch sensor pushed')

        time.sleep(period)