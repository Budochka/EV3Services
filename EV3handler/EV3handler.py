import json
import logging
import pika
import threading
import VoiceHandler
import MoveHandler
import SensorsHandler

def callback(ch, method, properties, body):
    if (method.routing_key.find('voice.wav') != -1):
        VoiceHandler.callback_voice(ch, method, properties, body)
        return

    if (method.routing_key.find('movement.') != -1):
        MoveHandler.callback_move(ch, method, properties, body)
        return

def main_loop(name):
    logging.info('Main thread started')
    print('Main thread started')

    #read config file
    with open('config.json') as json_file:
        data = json.load(json_file)
        rabbit_host = data['RabbitHost']
        rabbit_port = data['RabbitPort']
        rabbit_login = data['RabbitUserName']
        rabbit_psw = data['RabbitPassword']
        update_period = data['UpdatePeriod']

    #connect to rabbitmq
    connection = pika.BlockingConnection(pika.ConnectionParameters(host=rabbit_host, port=rabbit_port, credentials=pika.PlainCredentials(rabbit_login, rabbit_psw)))
    channel = connection.channel()
    channel.exchange_declare(exchange='EV3', exchange_type='topic', auto_delete=True)

    SensorsHandler.sensors_reporter(channel, update_period)

if __name__ == "__main__":
    #read config file
    with open('config.json') as json_file:
        data = json.load(json_file)
        rabbit_host = data['RabbitHost']
        rabbit_port = data['RabbitPort']
        rabbit_login = data['RabbitUserName']
        rabbit_psw = data['RabbitPassword']
        logfile = data['LogFileName']
    print('Reading configuration - done')

    #setup logging
    logging.basicConfig(filename=logfile, level=logging.INFO)

    #connect to rabbitmq
    connection = pika.BlockingConnection(pika.ConnectionParameters(host=rabbit_host, port=rabbit_port, credentials=pika.PlainCredentials(rabbit_login, rabbit_psw)))
    channel = connection.channel()
    channel.exchange_declare(exchange='EV3', exchange_type='topic', auto_delete=True)
    logging.info('Rabbit connection created')

    result = channel.queue_declare(queue='')
    queue_name = result.method.queue
    print('Connecting to Rabbit - done')

    x = threading.Thread(target=main_loop, args=(1,))
    x.start()

    channel.queue_bind(exchange='EV3', routing_key='voice.wav', queue=queue_name)
    logging.info('Voice queue bind complete')

    channel.queue_bind(exchange='EV3', routing_key='movement.*', queue=queue_name)
    logging.info('Movement queue bind complete')

    channel.basic_consume(queue=queue_name, on_message_callback=callback, auto_ack=True)
    print('Listening for the events')
    channel.start_consuming()

    input("Press Enter to exit...")