using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;
namespace Services.RabbitMQ
{
    /// <summary>
    /// RabbitMQ 消息发送帮助
    /// </summary>
    public class RabbitMQClient
    {
        /// <summary>
        /// 信道
        /// </summary>
        private readonly IModel _channel;
        /// <summary>
        /// NLog 日志
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        public RabbitMQClient(ILogger<RabbitMQClient> logger)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = "xx.xxx.x.xx",
                    UserName = "admin",
                    Password = "admin",
                    Port = 5672,
                    VirtualHost = "test"
                };
                factory.AutomaticRecoveryEnabled = true;//自动连接恢复
                var connection = factory.CreateConnection();
                _channel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                logger.LogError(-1, ex, "RabbitMQClient init fail");
            }
            _logger = logger;
        }

        public virtual void PushMessage(string routingKey, object message,string exchange)
        {
            //订阅模式下，一般有消费者创建队列，生产者只负责发消息到交换机，至于消息该怎么发，以及发送到哪个队列，生产者都不负责
            //_channel.QueueDeclare(queue: queue,
            //                            durable: false,
            //                            exclusive: false,
            //                            autoDelete: false,
            //                            arguments: null);
            string msgJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(msgJson);
            //为了保证不报错，生产者和消费者都声明交换机，同样的，交换机的创建会保证幂等性
            _channel.ExchangeDeclare(exchange: exchange, type: "topic");
            _channel.BasicPublish(exchange: exchange,
                                    routingKey: routingKey,
                                    basicProperties: null,
                                    body: body);
           // System.Threading.Thread.Sleep(1);//此行代码必须，不然写入队列消息会报错
        }
    }

}
