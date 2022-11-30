import logging
import pika
import threading
import Utils
import MoveHandler
import VoiceHandler

#connecting to Rabbit using information from global config file
def connectToRabbit():
    connection = pika.BlockingConnection(pika.ConnectionParameters(host=config.rabbit_host, port=config.rabbit_port, 
                                         credentials=pika.PlainCredentials(config.rabbit_login, config.rabbit_psw)))
    channel = connection.channel()
    channel.exchange_declare(exchange='EV3', exchange_type='topic', auto_delete=True)

    logging.info('Rabbit connection created')

    return connection, channel

#callback for handling messages from Rabbit
def callback(ch, method, properties, body):
    if (method.routing_key.find('voice.wav') != -1):
        VoiceHandler.callback_voice(ch, method, properties, body)
        return

    if (method.routing_key.find('movement.') != -1):
        MoveHandler.callback_move(ch, method, properties, body)
        return

#program entry poitnt
if __name__ == "__main__":
    config = Utils("config.json")

    #setup logging
    logging.basicConfig(level=logging.INFO,
                        format='%(asctime)s %(levelname)s %(module)s %(funcName)s %(message)s',
                        handlers=[logging.FileHandler(config.logfile, mode='w'),
                        logging.StreamHandler()])

    connection, channel = connectToRabbit()

    result = channel.queue_declare(queue='')
    queue_name = result.method.queue
    print('Connecting to Rabbit - done')

    channel.queue_bind(exchange='EV3', routing_key='voice.wav', queue=queue_name)
    logging.info('Voice queue bind complete')

    channel.queue_bind(exchange='EV3', routing_key='movement.*', queue=queue_name)
    logging.info('Movement queue bind complete')

    channel.basic_consume(queue=queue_name, on_message_callback=callback, auto_ack=True)
    print('Listening for the events')
    channel.start_consuming()

    input("Press Enter to exit...")
