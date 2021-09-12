import pika
import threading
import os
#from ev3dev2.sound import Sound

def callback(ch, method, properties, body):
    wav_file = open('ToSpeak.wav', 'wb')
    wav_file.write(body)
    wav_file.close

#    spkr = Sound()
#    spkr.play_file('ToSpeak.wav')
    playsound('ToSpeak.wav')

    os.remove('ToSpeak.wav')
