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
                factory.HostName = "192.168.0.102";
                factory.Port = 25672;


                IConnection conn = factory.CreateConnection();
                IModel channel = conn.CreateModel();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}