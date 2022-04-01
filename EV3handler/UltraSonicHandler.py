import logging
import time
from ev3dev2.sensor import *

def DistanceReporter(channel, period):
    us = UltrasonicSensor()
    us.mode = 'US-DIST-CM'

    finished = False

    while not finished:
        distance = us.value() / 10 #convert to cm
        channel.basic_publish(exchange = 'EV3', routing_key = "sensors.distance", body= distance)
        logging.info('Distance sent ' + str(distance))
        time.sleep(1)