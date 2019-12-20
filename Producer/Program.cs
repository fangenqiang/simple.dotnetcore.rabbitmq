using RabbitMQ.Client;
using System;
using System.Text;

namespace Producer
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
        const string QueueName = "";
        //路由
        const string RoutingKey = "simple.dotnetcore.rabbitmq";


        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to RabbitMQ Product!");
            Publish(ExchangeType.Direct);
            Publish(ExchangeType.Topic);
            Publish(ExchangeType.Fanout);
            Console.WriteLine("按任意值，退出程序");
            Console.ReadKey();
        }

        public static void Publish(string exchangeType )
        {
            using (IConnection conn = rabbitMqFactory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    //声明交换机
                    channel.ExchangeDeclare(ExchangeName, exchangeType, durable: true, autoDelete: false, arguments: null);
                    //声明队列
                    channel.QueueDeclare(QueueName, durable: true, autoDelete: false, exclusive: false, arguments: null);
                    //绑定
                    channel.QueueBind(QueueName, ExchangeName, RoutingKey);
                    var properties = channel.CreateBasicProperties();
                    //队列持久化
                    properties.Persistent = true;
                    string vadata = Console.ReadLine();
                    while (vadata != "exit")
                    {
                        var msgBody = Encoding.UTF8.GetBytes(vadata);
                        channel.BasicPublish(exchange: ExchangeName, routingKey: RoutingKey, basicProperties: properties, body: msgBody);
                        Console.WriteLine(string.Format("***发送时间:{0}，发送完成，输入exit退出消息发送",
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                        vadata = Console.ReadLine();
                    }
                }
            }
        }
    }
}
