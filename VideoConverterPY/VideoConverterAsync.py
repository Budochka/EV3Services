from pickle import TRUE
import sys
import cv2
import pika
import threading
import time
import json
import logging
from cv2 import VideoCapture

class FrameSkipper:
    def __init__(self):
        self.sending = False
        self.is_finished = False
        self.flag_lock = threading.Lock()

    def set_flag(self):
        with self.flag_lock:
            self.sending = True

    def clear_flag(self):
        with self.flag_lock:
            self.sending = False

    def get_flag(self):
        with self.flag_lock:
            return self.sending


def donothing(sec, flag):
    while (not flag.is_finished):
        time.sleep(sec)
        flag.set_flag()

def process_frames(d, flag):
    try:
        #connect to rabbitmq
        connection = pika.BlockingConnection(
            pika.ConnectionParameters(
                host=rabbit_host, 
                port=rabbit_port, 
                credentials=pika.PlainCredentials(rabbit_login, rabbit_psw)
            )
        )
        channel = connection.channel()
        channel.exchange_declare(exchange='EV3', exchange_type='topic', auto_delete=True)
        logging.info('Rabbit connection created')

        #get video stream
        cap = VideoCapture(video_source)

        while(cap.isOpened() and not flag.is_finished):
            ret, frame = cap.read()
        
            #check if we got the image
            if not ret:
                logging.warning('No frame received')
                break

            if flag.get_flag():
                #resize image to 640*480, but preserve scale
                scale = 640 / frame.shape[1]
                resized = cv2.resize(frame, (640, int(frame.shape[0] * scale)), interpolation = cv2.INTER_LINEAR)
                res, image = cv2.imencode('.jpg', resized)

                #publish message
                if res:
                    msg = bytearray(image)
                    channel.basic_publish(exchange='EV3', routing_key="images.general", body=msg)
                    logging.info('Frame sent to rabbit')
                    flag.clear_flag()
        cap.release()

    except Exception as e:
        logging.exception("Error in process_frames: %s", e)


if __name__ == "__main__":
    #read config file
    with open('config.json') as json_file:
        data = json.load(json_file)
        rabbit_host = data['RabbitHost']
        rabbit_port = data['RabbitPort']
        rabbit_login = data['RabbitUserName']
        rabbit_psw = data['RabbitPassword']
        logfile = data['LogFileName']
        video_source = data['RTMPSource']
        delay = data['DelayBetweenFrames']

    #setup logging
    logging.basicConfig(level=logging.INFO,
                    format='%(asctime)s | %(levelname)s | %(module)s | %(funcName)s | %(message)s',
                    handlers=[
                        logging.FileHandler(logfile, mode='w'),
                        logging.StreamHandler(sys.stdout)
                    ])
    #create class
    frameskiper = FrameSkiper()

    #initialise thread
    delay_thread = threading.Thread(target=donothing, args=(delay,frameskiper), daemon=True)
    delay_thread.start()

    worker_thread = threading.Thread(target=process_frames, args=(0, frameskiper), daemon=True)
    worker_thread.start()

    input("Press Enter to exit...\n")

    frameskiper.is_finished = True
