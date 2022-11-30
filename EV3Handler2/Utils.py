import logging
import json
import pika

global config

#connecting to Rabbit using information from global config file
def connectToRabbit():
    connection = pika.BlockingConnection(pika.ConnectionParameters(host=config.rabbit_host, port=config.rabbit_port, 
                                         credentials=pika.PlainCredentials(config.rabbit_login, config.rabbit_psw)))
    channel = connection.channel()
    channel.exchange_declare(exchange='EV3', exchange_type='topic', auto_delete=True)

    logging.info('Rabbit connection created')

    return connection, channel

#global configuration
class Config:
    def __init__(self) :
        #read json file
        with open('config.json') as json_file:
            data = json.load(json_file)
            self.rabbit_host = data['RabbitHost']
            self.rabbit_port = data['RabbitPort']
            self.rabbit_login = data['RabbitUserName']
            self.rabbit_psw = data['RabbitPassword']
            self.update_period = data['UpdatePeriod']
            self.logfile = data['LogFileName']
        logging.info("Config class initialized successfully")

        stopped = False