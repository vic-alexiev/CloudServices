using IronMQ;
using IronMQUtils;
using Newtonsoft.Json;
using System;
using System.Configuration;

internal class IronMQSender
{
    private static IQueue queue;

    static IronMQSender()
    {
        string projectId = ConfigurationManager.AppSettings["IRON_MQ_PROJECT_ID"];
        var token = ConfigurationManager.AppSettings["IRON_MQ_TOKEN"];
        var client = new Client(projectId, token);
        queue = client.Queue("IronMQChat");
    }

    private static void SendMessage()
    {
        string text = Console.ReadLine();

        string host = IPAddressManager.GetLocalIPAddress();
        var message = new ChatMessage
        {
            SendingHost = host,
            Text = text
        };

        queue.Push(JsonConvert.SerializeObject(message));
    }

    private static void Main()
    {
        Console.WriteLine("Send a message");

        while (true)
        {
            SendMessage();
        }
    }
}
