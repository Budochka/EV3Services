import logging
import sys
import threading
import MoveHandler
import VoiceHandler
import SensorsHandler
import Utils

#callback for handling messages from Rabbit
def callback_messages(ch, method, properties, body):
    if (method.routing_key.find('voice.wav') != -1):
        VoiceHandler.callback_voice(ch, method, properties, body)
        return

    if (method.routing_key.find('movement.') != -1):
        MoveHandler.callback_move(ch, method, properties, body)
        return

#consumer callback
def consumer():
    connection, channel = Utils.connectToRabbit()

    result = channel.queue_declare(queue='')
    queue_name = result.method.queue

    channel.queue_bind(exchange='EV3', routing_key='voice.wav', queue=queue_name)
    logging.info('Voice queue bind complete')

    channel.queue_bind(exchange='EV3', routing_key='movement.*', queue=queue_name)
    logging.info('Movement queue bind complete')

    channel.basic_consume(queue=queue_name, on_message_callback=callback_messages, auto_ack=True)
    logging.info('Listening for the events')
    channel.start_consuming()
    

#program entry poitnt
if __name__ == "__main__":
    Utils.initialize()

    #setup logging
    logging.basicConfig(level=logging.INFO,
                        format='%(asctime)s %(levelname)s %(module)s %(funcName)s %(message)s',
                        handlers=[
                            logging.FileHandler(Utils.global_config.logfile),
                            logging.StreamHandler(sys.stdout)
                        ])

    consumer_thread = threading.Thread(target=consumer)
    consumer_thread.daemon = True
    consumer_thread.start()
    logging.info('Consumer thread statrted')

    touch_thread = threading.Thread(target=SensorsHandler.touchensor_reporter)
    touch_thread.start()
    logging.info('Touch sensor thread statrted')

    sensors_thread = threading.Thread(target=SensorsHandler.sensors_reporter)
    sensors_thread.start()
    logging.info('Distanse & color sensors thread statrted')

    input("Press Enter to exit...\n")

    Utils.global_config.stopped = True
    