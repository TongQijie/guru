using System;
using RabbitMQ.Client;

namespace ConsoleApp.Mq
{
    public class TestRunner
    {
        public void Run()
        {
            try
            {
                var factory = new ConnectionFactory();
                factory.UserName = "jerry";
                factory.Password = "123456";
                //factory.VirtualHost = "/";
                factory.HostName = "192.168.0.101";
                factory.Port = 5672;
                factory.Protocol = Protocols.DefaultProtocol;

                using (IConnection conn = factory.CreateConnection())
                {
                    IModel channel = conn.CreateModel();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}