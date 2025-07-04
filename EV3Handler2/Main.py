import logging
import sys
import threading
import MoveHandler
import VoiceHandler
import SensorsHandler
import Utils

shutdown_event = threading.Event()

def callback_messages(ch, method, properties, body):
    try:
        if method.routing_key == 'voice.wav':
            VoiceHandler.callback_voice(ch, method, properties, body)
        elif method.routing_key.startswith('movement.'):
            MoveHandler.callback_move(ch, method, properties, body)
    except Exception as e:
        logging.exception("Error in callback_messages: %s", e)

def consumer():
    try:
        connection, channel = Utils.connectToRabbit()
        result = channel.queue_declare(queue='')
        queue_name = result.method.queue

        channel.queue_bind(exchange='EV3', routing_key='voice.wav', queue=queue_name)
        logging.info('Voice queue bind complete')

        channel.queue_bind(exchange='EV3', routing_key='movement.*', queue=queue_name)
        logging.info('Movement queue bind complete')

        channel.basic_consume(queue=queue_name, on_message_callback=callback_messages, auto_ack=True)
        logging.info('Listening for the events')
        while not shutdown_event.is_set():
            channel.connection.process_data_events(time_limit=1)
    except Exception as e:
        logging.exception("Error in consumer: %s", e)

def run_sensor_reporter(reporter_func, name):
    try:
        reporter_func(shutdown_event)
    except Exception as e:
        logging.exception(f"Error in {name}: %s", e)

if __name__ == "__main__":
    # Setup logging before initialization if possible
    logging.basicConfig(level=logging.INFO,
                        format='%(asctime)s %(levelname)s %(module)s %(funcName)s %(message)s',
                        handlers=[
                            logging.FileHandler("ev3handler.log", mode='w'),
                            logging.StreamHandler(sys.stdout)
                        ])
    Utils.initialize()

    consumer_thread = threading.Thread(target=consumer, name="ConsumerThread")
    consumer_thread.start()
    logging.info('Consumer thread started')

    touch_thread = threading.Thread(target=run_sensor_reporter, args=(SensorsHandler.touchensor_reporter, "TouchSensor"), name="TouchSensorThread")
    touch_thread.start()
    logging.info('Touch sensor thread started')

    sensors_thread = threading.Thread(target=run_sensor_reporter, args=(SensorsHandler.sensors_reporter, "Sensors"), name="SensorsThread")
    sensors_thread.start()
    logging.info('Distance & color sensors thread started')

    input("Press Enter to exit...\n")
    shutdown_event.set()

    # Join threads for graceful shutdown
    consumer_thread.join()
    touch_thread.join()
    sensors_thread.join()