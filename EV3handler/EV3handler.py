import json
import logging
import pika
import VoiceHandler

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
    logging.basicConfig(filename=logfile, level=logging.INFO)

    #connect to rabbitmq
    connection = pika.BlockingConnection(pika.ConnectionParameters(host=rabbit_host, port=rabbit_port, credentials=pika.PlainCredentials(rabbit_login, rabbit_psw)))
    channel = connection.channel()
    channel.exchange_declare(exchange='EV3', exchange_type='topic', auto_delete=True)
    logging.info('Rabbit connection created')

    result = channel.queue_declare(queue='')
    queue_name = result.method.queue

    channel.queue_bind(exchange='EV3', routing_key='voice.generated.wav', queue=queue_name)
    channel.basic_consume(queue=queue_name, on_message_callback=VoiceHandler.callback, auto_ack=True)

    input("Press Enter to exit...")