using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer2
{
    class Program
    {
        private static readonly ConnectionFactory rabbitMqFactory = new ConnectionFactory()
        {
            UserName = "fgq",
            Password = "123456",
            Port = 5672,
            //VirtualHost = "localhost",
            RequestedHeartbeat = 0,
            Endpoint = new AmqpTcpEndpoint(new Uri("amqp:http://192.168.2.250:15672/"))
        };

        //交换机名称
        const string ExchangeName = "simple.dotnetcore.rabbitmq.exchange";
        //队列名称
        const string QueueName = "simple.dotnetcore.rabbitmq.queue2";
        //路由
        const string RoutingKey = "simple.dotnetcore.rabbitmq.#";
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to RabbitMQ Consumer!");
            Receive(ExchangeType.Direct);
            Receive(ExchangeType.Topic);
            Receive(ExchangeType.Fanout);
            Console.WriteLine("按任意值，退出程序");
            Console.ReadKey();
        }


        public static void Receive(string exchangeType)
        {
            using (IConnection conn = rabbitMqFactory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    channel.ExchangeDeclare(ExchangeName, exchangeType, durable: true, autoDelete: false, arguments: null);
                    channel.QueueDeclare(QueueName, durable: true, autoDelete: false, exclusive: false, arguments: null);
                    channel.QueueBind(QueueName, ExchangeName, routingKey: RoutingKey);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) =>
                    {
                        var msgBody = Encoding.UTF8.GetString(ea.Body);
                        Console.WriteLine(string.Format("队列2接收时间:{0}，消息内容：{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msgBody));
                    };
                    channel.BasicConsume(QueueName, true, consumer: consumer);
                    Console.WriteLine("按任意值，退出程序");
                    Console.ReadKey();
                }
            }
        }
    }
}
