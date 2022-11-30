import logging
import time
import Utils

from struct import *
from ev3dev2.sensor.lego import ColorSensor, TouchSensor, UltrasonicSensor

def touchensor_reporter():
    ts = TouchSensor()
    logging.info("TouchSensor is started")

    connection, channel = Utils.connectToRabbit()

    while not Utils.config.stopped:
        if ts.is_pressed:
            channel.basic_publish(exchange = 'EV3', routing_key = "sensors.touch", body=pack('b', 1))
            logging.info('Touch sensor pushed')

    logging.info("TouchSensor stopped")

    
def sensors_reporter():
    us = UltrasonicSensor()
    # Put the US sensor into distance mode.
    us.mode = 'US-DIST-CM'

    cl = ColorSensor()
    # Put the color sensor into RGB mode.
    cl.mode='COL-COLOR'
    
    logging.info("Color & Ultrasonic are started")

    connection, channel = Utils.connectToRabbit()

    while not Utils.config.stopped:
        distance = us.distance_centimeters
        channel.basic_publish(exchange = 'EV3', routing_key = "sensors.distance", body=pack('f', distance))
        logging.info('Distance is ' + str(distance))

        color = cl.color_name;
        channel.basic_publish(exchange = 'EV3', routing_key = "sensors.color", body=pack('b', cl.color))
        logging.info('Colors is ' + color)

        time.sleep(Utils.config.update_period)