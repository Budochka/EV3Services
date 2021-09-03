import cv2
import pika
import threading
import time

class FrameSkiper:
    def __init__(self):
        self.sending = False
        self.read_lock = threading.Lock()
        self.write_lock = threading.Lock()        

    def set_flag(self):
        self.write_lock.acquire()
        self.sending = True
        self.write_lock.release()

    def clear_flag(self):
        self.write_lock.acquire()
        self.sending = False
        self.write_lock.release()

    def get_flag(self):
        self.read_lock.acquire()
        flag = self.sending
        self.read_lock.release()
        return flag


def donothing(sec, flag):
    while (True):
        time.sleep(sec)
        flag.set_flag()


if __name__ == "__main__":
    #create class
    frameskiper = FrameSkiper()

    #initialise thread
    x = threading.Thread(target=donothing, args=(3,frameskiper), daemon=True)
    x.start()

    #connect to rabbitmq
    connection = pika.BlockingConnection(pika.ConnectionParameters(host='localhost'))
    channel = connection.channel()
    channel.exchange_declare(exchange='EV3', exchange_type='topic', auto_delete=True)

    #get video stream
    cap = cv2.VideoCapture('rtmp://demo.flashphoner.com:1935/live/rtmp_9cd0')

    while(cap.isOpened()):
        ret, frame = cap.read()
        
        #check if we got the image
        if not ret:
            break

        if frameskiper.get_flag():
            #resize image to 640*480, but preserve scale
            scale = 640 / frame.shape[1]
            resized = cv2.resize(frame, (640, int(frame.shape[0] * scale)), interpolation = cv2.INTER_AREA)

            #publish message
            msg = bytearray(resized)
            channel.basic_publish(exchange='EV3', routing_key="images.general", body=msg)
            frameskiper.clear_flag()

    cap.release()
