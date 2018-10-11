using AMSPOC.Models;
using Apache.NMS;
using Apache.NMS.Util;
using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using Apache.NMS.ActiveMQ;


namespace ActiveMQSubscriber
{
    class Program
    {
        
        static void Main(string[] args)
        {
            IConnectionFactory factory = new ConnectionFactory("activemq:tcp://localhost:61616");
            IConnection _connection = factory.CreateConnection();
            _connection.Start();
            ISession _session = _connection.CreateSession();

            IDestination dest1 = _session.GetQueue("TECHM.BAR");
            IMessageConsumer consumer = _session.CreateConsumer(dest1);
            consumer.Listener += ConsumerQ1_Listener;

            IDestination dest2 = _session.GetQueue("TECHM.BAR1");
            IMessageConsumer consumer2 = _session.CreateConsumer(dest2);
            consumer2.Listener += ConsumerQ2_Listener;

            IDestination dest3 = _session.GetQueue("TECHM.BAR2");
            IMessageConsumer consumer3 = _session.CreateConsumer(dest3);
            consumer3.Listener += ConsumerQ3_Listener;
            
            Console.ReadLine();
        }

        private static void ConsumerQ3_Listener(IMessage message)
        {
            //Read from MQ
            ServicePoint opldObject = Newtonsoft.Json.JsonConvert.DeserializeObject<ServicePoint>((message as ITextMessage).Text);

            string myString = Newtonsoft.Json.JsonConvert.SerializeObject(opldObject);

            HttpClient client = new HttpClient();
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(opldObject));
            System.Threading.Tasks.Task<HttpResponseMessage> response = client.PostAsync("http://localhost:59477/api/ServicePoint/ReadMQ3ServicePointNWriteToDB?strServicepoint=" + myString, content);
        }

        private static void ConsumerQ2_Listener(IMessage message)
        {
            //Read from MQ
            OPLD opldObject = Newtonsoft.Json.JsonConvert.DeserializeObject<OPLD>((message as ITextMessage).Text);

            string myString = Newtonsoft.Json.JsonConvert.SerializeObject(opldObject);

            HttpClient client = new HttpClient();
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(opldObject));
            System.Threading.Tasks.Task<HttpResponseMessage> response = client.PostAsync("http://localhost:59477/api/ServicePoint/ReadMQ2OPLDNCreateSPNPushTOMQ3?strOPLD=" + myString, content);
        }

        private static void ConsumerQ1_Listener(IMessage message)
        {
            //Read from MQ
            OPLD opldObject = Newtonsoft.Json.JsonConvert.DeserializeObject<OPLD>((message as ITextMessage).Text);

            string myString = Newtonsoft.Json.JsonConvert.SerializeObject(opldObject);

            HttpClient client = new HttpClient();
            var content = new  StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(opldObject));
            System.Threading.Tasks.Task<HttpResponseMessage> response = client.PostAsync("http://localhost:59477/api/ProcessOPLD/ProcessMQ1OPLDMessageNWriteToDBNMQ2?strOPLD="+myString, content);
        }
    }
}
