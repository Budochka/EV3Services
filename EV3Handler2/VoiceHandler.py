import os
import logging
from ev3dev2.sound import Sound

def callback_voice(ch, method, properties, body):
    wav_file = open('ToSpeak.wav', 'wb')
    wav_file.write(body)
    wav_file.close

    spkr = Sound()
    spkr.set_volume(100, channel=None)
    spkr.play_file('ToSpeak.wav')

    os.remove('ToSpeak.wav')
    logging.info('Voice message processed')
