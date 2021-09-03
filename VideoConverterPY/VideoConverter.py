import numpy as np
import cv2
import pika

#connect to rabbitmq
connection = pika.BlockingConnection(pika.ConnectionParameters(host='localhost'))
channel = connection.channel()
channel.exchange_declare(exchange='EV3', exchange_type='topic', auto_delete=True)

#get video stream
cap = cv2.VideoCapture('rtmp://demo.flashphoner.com:1935/live/rtmp_b883')

while(cap.isOpened()):
    ret, frame = cap.read()
    
    #check if we got the image
    if not ret:
        break

    #resize image to 640*480, but preserve scale
    scale = 640 / frame.shape[1]
    resized = cv2.resize(frame, (640, int(frame.shape[0] * scale)), interpolation = cv2.INTER_AREA)
    cv2.imshow('frame', resized)

    #publish message
    msg = bytearray(resized)
    #channel.basic_publish(exchange='EV3', routing_key="images.general", body=msg)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()