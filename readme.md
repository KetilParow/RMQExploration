# Findings

## Setup
- Log files reside at C:\Users\...\AppData\Roaming\RabbitMQ\log
- Database: At C:\Users\...\AppData\Roaming\RabbitMQ\db
  - Quorum queue storage: C:\Users\PCADMIN\AppData\Roaming\RabbitMQ\db\{clustername}-mnesia\quorum\rabbit@UTV-5CG0388TLJ
- config file: At C:\Users\...\AppData\Roaming\RabbitMQ\advanced.config. See https://www.rabbitmq.com/configure.html#advanced-config-file and https://github.com/rabbitmq/rabbitmq-server/blob/v3.8.x/deps/rabbit/docs/rabbitmq.conf.example
- 

## Tests 
   - Issues
     - CLOSE and DISPOSE of receivers may lead to errors:
       - Fails quietly on quorum queues (errors logged)
       - Throws exception on in-memory queues.
       - wut?