using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;
namespace Services.RabbitMQ
{
    /// <summary>
    /// RabbitMQ 消息队列监听类
    /// </summary>
    public class RabbitListener : IHostedService
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private IConfiguration configuration;

        public RabbitListener()
        {
            try
            {
                //创建连接工厂
                var factory = new ConnectionFactory()
                {
                    HostName = "xx.xxx.xx.xxx",
                    UserName = "admin",
                    Password = "admin",
                    Port = 5672,
                    VirtualHost = "test"
                };
           
                this.connection = factory.CreateConnection();
                this.channel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RabbitListener init error,ex:{ex.Message}");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Register();
            return Task.CompletedTask;
        }



        
        /// <summary>
        /// 路由
        /// </summary>
        protected string RouteKey;
        /// <summary>
        /// 队列名字
        /// </summary>
        protected string QueueName;
        /// <summary>
        /// 交换机
        /// </summary>
        protected string Exchange;
        // 处理消息的方法
        public virtual bool Process(string message)
        {
            throw new NotImplementedException();
        }

        // 注册消费者监听在这里
        public void Register()
        {
            //Console.WriteLine($"RabbitListener register,routeKey:{RouteKey}");
            //定义一个Direct类型交换机
            
            channel.ExchangeDeclare(exchange: Exchange, type: "topic");
            //定义一个队列
            //channel.QueueDeclare(queue: QueueName, exclusive: false);
            channel.QueueDeclare(QueueName, false, false, false, null);
            //将队列绑定到交换机
            channel.QueueBind(queue: QueueName,
                              exchange: Exchange,
                              routingKey: RouteKey);
            //事件基本消费者
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var result = Process(message);
                if (result)
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            channel.BasicConsume(queue: QueueName, consumer: consumer);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void DeRegister()
        {
            this.connection.Close();
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.channel.Close();
            this.connection.Close();
            return Task.CompletedTask;
        }
    }


}
