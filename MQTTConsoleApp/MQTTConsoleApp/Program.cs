using System;
using System.IO;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Threading;

namespace MQTTConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            client.Start();
            Console.ReadKey();
        }
    }

    class client
    {
        static MqttClient Client;
        public static void Start()
        {

            Client = new MqttClient("cansat.info");   

            string username = "t1010";
            string password = "t1010pass";
            string topic = "teams/1010";
            string filePath = Path.GetFullPath("cansat1.csv");
            byte code = Client.Connect(Guid.NewGuid().ToString(),username,password);

            StreamReader reader = new StreamReader(filePath);

            Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            if (code == 0x00)
            {
                Console.WriteLine("Client connected to Server node!");
            }
            else
            {
                Console.WriteLine("Connection Refused");
            }
            try
            {
                Client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        string[] values = line.Split(',');
                        if(values.Length > 1)
                        {
                            if(values[3] == "C")
                            {
                                Thread.Sleep(1000);
                            }
                        }
                        Client.Publish(topic, Encoding.UTF8.GetBytes(line), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
                        Console.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Start: Exception thrown: " + ex.Message);
            }
        }

        static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //Console.WriteLine(System.Text.Encoding.Default.GetString(e.Message));
            //Console.WriteLine(e.Topic);
        }
    }
}
